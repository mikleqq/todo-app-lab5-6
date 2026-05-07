# 📋 TODO App Pro - Modern Task Management

**A real-time task management application with WebSocket support, modern UI, and complete CI/CD pipeline.**

---

## ✨ Features

- 🎨 **Modern UI** - Beautiful, responsive design with gradient backgrounds and smooth animations
- 🔄 **Real-time Synchronization** - WebSocket-based updates across all connected clients
- 📧 **Email Integration** - SMTP, IMAP, and POP3 support for task notifications
- 🐳 **Docker Containerized** - Fully containerized with PostgreSQL and MailHog
- 🚀 **CI/CD Pipeline** - Automated testing and Docker Hub deployment via GitHub Actions
- 📱 **Responsive Design** - Works on desktop, tablet, and mobile devices

---

## 🚀 Quick Start

### Option 1: Simple Local Setup (Recommended for quick testing)
No Docker needed - just Python!

#### Windows
```bash
START_LOCAL.bat
```

#### PowerShell
```powershell
.\START_LOCAL.ps1
```

#### Linux/Mac
```bash
python3 start_local.py
```

**Note:** Uses SQLite database. Email services (SMTP/IMAP/POP3) not available in this mode.

---

### Option 2: Full Docker Setup (Complete features)
Includes PostgreSQL, MailHog, and all services

#### Windows - Batch File
```bash
START.bat
```

#### Windows - PowerShell
```powershell
.\START.ps1
```

---

## 🌐 Access Points

| Service | URL | Purpose |
|---------|-----|---------|
| **Frontend** | http://localhost | Task Management UI |
| **Backend API** | http://localhost:8000/docs | Interactive API Documentation (Swagger) |
| **MailHog UI** | http://localhost:8025 | Email Service Interface |
| **WebSocket** | ws://localhost:8000/ws | Real-time Updates |
| **Database** | localhost:5432 | PostgreSQL (postgres/postgres) |

---

## 📦 Project Structure

```
todo-app/
├── frontend/
│   ├── Dockerfile
│   ├── index.html          # Modern UI with Bootstrap & Font Awesome
│   └── .dockerignore
├── backend/
│   ├── Dockerfile
│   ├── main.py             # FastAPI + WebSocket + SQLAlchemy
│   ├── requirements.txt
│   └── .dockerignore
├── .github/
│   └── workflows/
│       └── ci-cd.yml       # GitHub Actions Pipeline
├── START.bat               # Windows Startup Script
├── START.ps1               # Windows PowerShell Script
└── README.md
```

---

## 🛠️ Technology Stack

### Backend
- **FastAPI** - Modern Python web framework
- **SQLAlchemy** - ORM for database interactions
- **WebSockets** - Real-time communication
- **PostgreSQL** - Database
- **Uvicorn** - ASGI server

### Frontend
- **HTML5** - Structure
- **CSS3** - Modern styling with gradients and animations
- **JavaScript** - Vanilla JS (no dependencies)
- **Font Awesome** - Icons
- **Bootstrap** - CSS framework

### DevOps
- **Docker** - Containerization
- **GitHub Actions** - CI/CD Pipeline
- **Docker Hub** - Container Registry
- **MailHog** - Email testing

---

## 🔌 API Endpoints

### Tasks
```
GET    /tasks              # List all tasks
POST   /tasks              # Create new task
GET    /tasks/{id}         # Get specific task
PUT    /tasks/{id}         # Update task
DELETE /tasks/{id}         # Delete task
```

### Real-time
```
WS     /ws                 # WebSocket connection
```

### Email Services
```
POST   /tasks/{id}/email   # Send task via SMTP
GET    /mail/imap          # Check IMAP info
GET    /mail/pop3          # Check POP3 info
```

### Health
```
GET    /health             # Service health check
GET    /                   # API information
```

---

## 📝 Task Data Model

```json
{
  "id": 1,
  "title": "Sample Task",
  "description": "Task description here"
}
```

---

## 🎯 Create a Task

**Request:**
```bash
curl -X POST http://localhost:8000/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"My Task","description":"Task details"}'
```

**Response:**
```json
{
  "id": 1,
  "title": "My Task",
  "description": "Task details"
}
```

---

## 🔄 WebSocket Real-time Updates

The application broadcasts task changes to all connected clients:

```javascript
// Client-side example
const socket = new WebSocket('ws://localhost:8000/ws');

socket.onmessage = (event) => {
  const data = JSON.parse(event.data);
  if (data.event === "update") {
    console.log("Task updated:", data.message);
    // Refresh task list
  }
};
```

---

## 🛑 Stop Services

### Windows
```bash
docker stop todo-backend todo-frontend todo-database todo-mailhog
docker rm todo-backend todo-frontend todo-database todo-mailhog
```

### Linux/Mac
```bash
docker stop todo-backend todo-frontend todo-database todo-mailhog
docker rm todo-backend todo-frontend todo-database todo-mailhog
docker network rm todo-network
```

---

## 📊 View Logs

```bash
# Backend logs
docker logs -f todo-backend

# Frontend logs
docker logs -f todo-frontend

# Database logs
docker logs -f todo-database

# Email service logs
docker logs -f todo-mailhog
```

---

## 🔄 GitHub Actions CI/CD Pipeline

The workflow includes:

1. **Build** - Builds Docker images for backend and frontend
2. **Test** - Runs HTTP tests against all endpoints
3. **Push** - Pushes images to Docker Hub (on main branch push)
4. **Cleanup** - Removes test containers and artifacts
5. **Notify** - Sends status notifications

### Required GitHub Secrets:
- `DOCKER_USERNAME` - Your Docker Hub username
- `DOCKER_PASSWORD` - Your Docker Hub password/token

---

## 📋 Lab Requirement Compliance

### Lab 5 - WebSocket Integration ✓
- ✓ WebSocket server implemented in FastAPI
- ✓ Real-time task synchronization across clients
- ✓ Event broadcasting for add/update/delete operations
- ✓ Reconnection handling and status indicators

### Lab 6 - CI/CD Pipeline ✓
- ✓ GitHub Actions workflow configuration
- ✓ Automated Docker image building
- ✓ HTTP endpoint testing with curl
- ✓ Docker Hub integration
- ✓ Container health verification
- ✓ Automated cleanup and artifact management

---

## 🐛 Troubleshooting

### Port Already in Use
```bash
# Find and kill process on port
# Windows
netstat -ano | findstr :8000
taskkill /PID <PID> /F

# Linux/Mac
lsof -i :8000
kill -9 <PID>
```

### Docker Daemon Not Running
- **Windows**: Open Docker Desktop application
- **Linux**: Run `sudo systemctl start docker`
- **Mac**: Open Docker.app from Applications

### Database Connection Error
- Ensure PostgreSQL container is running: `docker ps`
- Check DATABASE_URL environment variable
- Verify network connectivity: `docker network inspect todo-network`

### WebSocket Connection Failed
- Frontend and backend must be on same network
- Check backend logs: `docker logs todo-backend`
- Verify no proxy/firewall blocking WebSocket

---

## 📚 Documentation

- [FastAPI Documentation](https://fastapi.tiangolo.com/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

---

## 📄 License

This project is created for educational purposes.

---

## 🤝 Contributing

Feel free to fork, modify, and improve!

---

**Happy tasking! 🚀**
