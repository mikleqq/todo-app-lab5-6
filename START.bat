@echo off
REM ==========================================
REM  TODO APP - All-in-One Startup Script
REM  Windows Batch File
REM ==========================================
REM This script starts all services needed for the ToDo application
REM with one click - Frontend, Backend, Database, and Email Service

echo.
echo ╔════════════════════════════════════════════╗
echo ║     TODO App Pro - Startup Script          ║
echo ║  Modern Task Management Application        ║
echo ╚════════════════════════════════════════════╝
echo.

REM Clean up any existing containers
echo [0/5] Cleaning up old containers...
docker stop todo-backend todo-frontend todo-database todo-mailhog >nul 2>&1
docker rm todo-backend todo-frontend todo-database todo-mailhog >nul 2>&1
echo ✓ Ready to start fresh

REM Check if Docker is installed
echo [1/6] Checking Docker installation...
docker --version >nul 2>&1
if errorlevel 1 (
    echo.
    echo ❌ ERROR: Docker is not installed or not in PATH
    echo.
    echo Please install Docker from: https://www.docker.com/products/docker-desktop
    echo After installation, restart your terminal and run this script again.
    echo.
    pause
    exit /b 1
)
echo ✓ Docker found: %DOCKER_HOST%

REM Check if docker daemon is running
echo.
echo [2/6] Checking Docker daemon status...
docker ps >nul 2>&1
if errorlevel 1 (
    echo ⚠ Docker daemon is not running. Starting Docker Desktop...
    start "" "C:\Program Files\Docker\Docker\Docker.exe"
    timeout /t 10
)
echo ✓ Docker daemon is running

REM Create Docker network
echo.
echo [3/6] Setting up Docker network...
docker network create todo-network 2>nul
echo ✓ Network ready

REM Start PostgreSQL Database
echo.
echo [3.1/5] Starting PostgreSQL Database...
docker run -d --name todo-database --network todo-network ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -e POSTGRES_DB=todo_db ^
  -p 5432:5432 ^
  postgres:15-alpine
echo ✓ Database started

REM Start MailHog (Email Service)
echo.
echo [3.2/5] Starting MailHog (Email Service)...
docker run -d --name todo-mailhog --network todo-network ^
  -p 1025:1025 ^
  -p 8025:8025 ^
  mailhog/mailhog:latest
echo ✓ MailHog started

REM Build Backend Image
echo.
echo [3.3/5] Building Backend image...
docker build -t todo-backend:latest ./backend
if errorlevel 1 (
    echo ❌ ERROR: Failed to build backend
    pause
    exit /b 1
)
echo ✓ Backend image built

REM Start Backend
echo.
echo [3.4/5] Starting Backend API...
docker run -d --name todo-backend --network todo-network ^
  -e DATABASE_URL=postgresql://postgres:postgres@todo-database:5432/todo_db ^
  -e SMTP_SERVER=todo-mailhog ^
  -e SMTP_PORT=1025 ^
  -p 8000:8000 ^
  todo-backend:latest
echo ✓ Backend started

REM Build Frontend Image
echo.
echo [3.5/5] Building Frontend image...
docker build -t todo-frontend:latest ./frontend
if errorlevel 1 (
    echo ❌ ERROR: Failed to build frontend
    pause
    exit /b 1
)
echo ✓ Frontend image built

REM Start Frontend (Port 8080 - no admin rights required on Windows)
echo.
echo [3.6/5] Starting Frontend...
docker run -d --name todo-frontend --network todo-network ^
  -p 8080:80 ^
  todo-frontend:latest
echo ✓ Frontend started

echo.
echo [5/6] Waiting for services to be ready...
echo Checking Backend API...
:backend_check
timeout /t 2 /nobreak
curl http://localhost:8000/health >nul 2>&1
if errorlevel 1 (
    echo  Retrying...
    goto backend_check
)
echo ✓ Backend API is ready

REM Open application in browser
echo.
echo [6/6] Opening application in browser...
timeout /t 2
start http://localhost:8080

echo.
echo ╔════════════════════════════════════════════╗
echo ║           ✓ All Systems Online             ║
echo ╚════════════════════════════════════════════╝
echo.
echo 📱 Frontend (Web Interface):
echo    → http://localhost:8080
echo.
echo 🔌 Backend API (Documentation):
echo    → http://localhost:8000/docs
echo.
echo 📧 Email Service (MailHog):
echo    → http://localhost:8025
echo.
echo 🗄️  Database:
echo    → PostgreSQL on localhost:5432
echo    → User: postgres / Password: postgres
echo.
echo 🔌 WebSocket:
echo    → ws://localhost:8000/ws
echo.
echo ℹ️  To stop all services, run:
echo    docker stop todo-backend todo-frontend todo-database todo-mailhog
echo.
echo ℹ️  To remove stopped containers, run:
echo    docker rm todo-backend todo-frontend todo-database todo-mailhog
echo.
echo ℹ️  To view logs, run:
echo    docker logs -f [container-name]
echo.
echo ℹ️  Available container names:
echo    - todo-backend
echo    - todo-frontend
echo    - todo-database
echo    - todo-mailhog
echo.
pause
