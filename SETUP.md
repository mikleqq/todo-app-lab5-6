# 🚀 Setup Guide - TODO App Pro

This guide provides detailed setup instructions for getting the TODO App running on your system.

---

## 📋 Prerequisites

Before you start, ensure you have:

1. **Docker & Docker Desktop**
   - [Download for Windows](https://www.docker.com/products/docker-desktop)
   - [Download for Mac](https://www.docker.com/products/docker-desktop)
   - [Installation Guide for Linux](https://docs.docker.com/engine/install/)

2. **Git** (optional, for cloning)
   - [Download Git](https://git-scm.com/downloads)

3. **Web Browser** (Chrome, Firefox, Edge, Safari, etc.)

---

## 🪟 Windows Setup

### Method 1: Automated Batch Script (Recommended)

**Step 1:** Open Command Prompt as Administrator
- Press `Win + X`
- Select "Command Prompt (Admin)" or "Windows PowerShell (Admin)"

**Step 2:** Navigate to project folder
```cmd
cd "path\to\todo-app"
```

**Step 3:** Run the startup script
```cmd
START.bat
```

**Step 4:** Wait for startup
- Script will check Docker installation
- Build and start all containers
- Open browser automatically
- Press Enter when complete

### Method 2: Manual PowerShell Script

**Step 1:** Open PowerShell as Administrator
- Press `Win + X`
- Select "Windows PowerShell (Admin)"

**Step 2:** Navigate to project folder
```powershell
cd "path\to\todo-app"
```

**Step 3:** Allow script execution
```powershell
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope CurrentUser
```

**Step 4:** Run the startup script
```powershell
.\START.ps1
```

### Method 3: Manual Docker Commands

**Step 1:** Open Command Prompt or PowerShell

**Step 2:** Create Docker network
```cmd
docker network create todo-network
```

**Step 3:** Start PostgreSQL
```cmd
docker run -d --name todo-database --network todo-network ^
  -e POSTGRES_USER=postgres ^
  -e POSTGRES_PASSWORD=postgres ^
  -e POSTGRES_DB=todo_db ^
  -p 5432:5432 ^
  postgres:15-alpine
```

**Step 4:** Start MailHog
```cmd
docker run -d --name todo-mailhog --network todo-network ^
  -p 1025:1025 ^
  -p 8025:8025 ^
  mailhog/mailhog:latest
```

**Step 5:** Build and start Backend
```cmd
docker build -t todo-backend:latest ./backend
docker run -d --name todo-backend --network todo-network ^
  -e DATABASE_URL=postgresql://postgres:postgres@todo-database:5432/todo_db ^
  -e SMTP_SERVER=todo-mailhog ^
  -e SMTP_PORT=1025 ^
  -p 8000:8000 ^
  todo-backend:latest
```

**Step 6:** Build and start Frontend
```cmd
docker build -t todo-frontend:latest ./frontend
docker run -d --name todo-frontend --network todo-network ^
  -p 80:80 ^
  todo-frontend:latest
```

**Step 7:** Open browser
```cmd
start http://localhost
```

---

## 🐧 Linux Setup

### Automated Script Setup

**Step 1:** Clone or navigate to project
```bash
cd ~/path/to/todo-app
```

**Step 2:** Create a startup script
```bash
cat > start.sh << 'EOF'
#!/bin/bash

echo "Creating Docker network..."
docker network create todo-network 2>/dev/null || true

echo "Starting PostgreSQL..."
docker run -d --name todo-database --network todo-network \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=todo_db \
  -p 5432:5432 \
  postgres:15-alpine

echo "Starting MailHog..."
docker run -d --name todo-mailhog --network todo-network \
  -p 1025:1025 \
  -p 8025:8025 \
  mailhog/mailhog:latest

echo "Building Backend..."
docker build -t todo-backend:latest ./backend

echo "Starting Backend..."
docker run -d --name todo-backend --network todo-network \
  -e DATABASE_URL=postgresql://postgres:postgres@todo-database:5432/todo_db \
  -e SMTP_SERVER=todo-mailhog \
  -e SMTP_PORT=1025 \
  -p 8000:8000 \
  todo-backend:latest

echo "Building Frontend..."
docker build -t todo-frontend:latest ./frontend

echo "Starting Frontend..."
docker run -d --name todo-frontend --network todo-network \
  -p 80:80 \
  todo-frontend:latest

echo "Waiting for services..."
sleep 5

echo "Opening browser..."
if command -v xdg-open &> /dev/null; then
  xdg-open http://localhost
elif command -v open &> /dev/null; then
  open http://localhost
fi

echo "✓ All services started!"
echo "Frontend: http://localhost"
echo "API Docs: http://localhost:8000/docs"
echo "MailHog: http://localhost:8025"
EOF
```

**Step 3:** Make it executable
```bash
chmod +x start.sh
```

**Step 4:** Run the script
```bash
./start.sh
```

### Manual Commands

```bash
# Create network
docker network create todo-network

# Start database
docker run -d --name todo-database --network todo-network \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=todo_db \
  -p 5432:5432 \
  postgres:15-alpine

# Start email service
docker run -d --name todo-mailhog --network todo-network \
  -p 1025:1025 \
  -p 8025:8025 \
  mailhog/mailhog:latest

# Build backend
docker build -t todo-backend:latest ./backend

# Start backend
docker run -d --name todo-backend --network todo-network \
  -e DATABASE_URL=postgresql://postgres:postgres@todo-database:5432/todo_db \
  -e SMTP_SERVER=todo-mailhog \
  -e SMTP_PORT=1025 \
  -p 8000:8000 \
  todo-backend:latest

# Build frontend
docker build -t todo-frontend:latest ./frontend

# Start frontend
docker run -d --name todo-frontend --network todo-network \
  -p 80:80 \
  todo-frontend:latest

# Open browser (Linux)
xdg-open http://localhost

# Open browser (Mac)
open http://localhost
```

---

## 🍎 Mac Setup

The Mac setup is identical to Linux. Use the Linux instructions above with these Mac-specific notes:

**Docker Installation:**
```bash
# Install via Homebrew (if you have Homebrew)
brew install --cask docker

# Or download from: https://www.docker.com/products/docker-desktop
```

**Starting Docker:**
```bash
# Docker usually starts automatically, but if needed:
open /Applications/Docker.app
```

**Browser commands:**
```bash
# Open browser on Mac
open http://localhost
```

---

## ✅ Verification

After setup, verify everything is running:

### 1. Check Containers
```bash
docker ps
```

You should see 4 containers:
- todo-backend
- todo-frontend
- todo-database
- todo-mailhog

### 2. Test Frontend
Open browser: http://localhost

### 3. Test Backend API
```bash
curl http://localhost:8000/health
```

Expected response:
```json
{"status":"healthy"}
```

### 4. Test WebSocket
Open browser console and check:
```javascript
const ws = new WebSocket('ws://localhost:8000/ws');
ws.onopen = () => console.log('Connected!');
```

### 5. Test MailHog
Open: http://localhost:8025

---

## 🛑 Stopping Services

### Windows & Linux & Mac
```bash
# Stop all containers
docker stop todo-backend todo-frontend todo-database todo-mailhog

# Remove containers
docker rm todo-backend todo-frontend todo-database todo-mailhog

# Remove network
docker network rm todo-network
```

### Remove volumes (delete database)
```bash
# Check volumes
docker volume ls

# Remove specific volume
docker volume rm [volume-name]
```

---

## 🔍 Troubleshooting

### Issue: Docker daemon not running
**Solution:**
- Windows: Open Docker Desktop
- Linux: `sudo systemctl start docker`
- Mac: Open Docker.app

### Issue: Port 80 already in use
**Solution:**
```bash
# Find what's using port 80
# Windows: netstat -ano | findstr :80
# Linux/Mac: lsof -i :80

# Kill the process or use different port:
docker run -p 8080:80 todo-frontend:latest
```

### Issue: Permission denied
**Solution:**
```bash
# Linux/Mac - run with sudo
sudo docker ps

# Or add user to docker group (Linux)
sudo usermod -aG docker $USER
newgrp docker
```

### Issue: WebSocket connection fails
**Check:**
- Backend is running: `docker logs todo-backend`
- Network is created: `docker network ls`
- Frontend can reach backend: `curl localhost:8000/health`

### Issue: Database connection error
**Check:**
- Database is running: `docker ps | grep postgres`
- Database logs: `docker logs todo-database`
- Network connectivity: `docker network inspect todo-network`

---

## 📝 Using the Application

1. **Create a Task**
   - Type title and description
   - Click "Add" button
   - Task appears in list

2. **View Task Details**
   - Click on any task title
   - Details panel appears on the right
   - Shows ID, title, description

3. **Edit a Task**
   - Click the pencil icon next to task
   - Fields are filled with task data
   - Click "Update" to save

4. **Delete a Task**
   - Click the trash icon next to task
   - Confirm deletion
   - Task is removed

5. **Email Services**
   - Click on a task
   - In details panel, use email buttons
   - SMTP, IMAP, POP3 options available

6. **Real-time Updates**
   - Open app in multiple browser windows
   - Make changes in one window
   - See instant updates in others
   - WebSocket status shown at top

---

## 📚 Next Steps

- Read [README.md](README.md) for full documentation
- Check [DEPLOYMENT.md](DEPLOYMENT.md) for CI/CD information
- Review API documentation: http://localhost:8000/docs
- Explore source code in `frontend/` and `backend/` directories

---

## 🆘 Need Help?

- Check logs: `docker logs [container-name]`
- Verify all containers running: `docker ps`
- Check network: `docker network inspect todo-network`
- Review README.md troubleshooting section

---

**Happy coding! 🚀**
