from fastapi import FastAPI, Depends, HTTPException, WebSocket, WebSocketDisconnect
from sqlalchemy import create_engine, Column, Integer, String
from sqlalchemy.orm import declarative_base, sessionmaker, Session  # Исправленный импорт
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List
import os
import json
import uvicorn  # Добавили для запуска через python main.py

# Импорты для работы с почтой
import smtplib
from email.mime.text import MIMEText

# 1. Настройка базы данных
# Для локальной разработки используем SQLite (не требует зависимостей)
# В Docker используется PostgreSQL (см. DATABASE_URL в Docker)
db_url = os.getenv("DATABASE_URL", "sqlite:///./test.db")

# Конфигурация для SQLite
if "sqlite" in db_url:
    engine = create_engine(db_url, connect_args={"check_same_thread": False})
else:
    # PostgreSQL конфигурация для Docker
    engine = create_engine(db_url)
SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)
Base = declarative_base()

# Настройки для MailHog (локально обычно localhost, если не в Docker)
SMTP_SERVER = os.getenv("SMTP_SERVER", "localhost") 
SMTP_PORT = int(os.getenv("SMTP_PORT", "1025"))

# 2. Модель таблицы в БД
class Task(Base):
    __tablename__ = "tasks"
    id = Column(Integer, primary_key=True, index=True)
    title = Column(String, index=True)
    description = Column(String, index=True)

# Создание таблиц при запуске
Base.metadata.create_all(bind=engine)

# 3. Схемы Pydantic
class TaskBase(BaseModel):
    title: str
    description: str

class TaskCreate(TaskBase):
    pass

class TaskResponse(TaskBase):
    id: int
    class Config:
        from_attributes = True

# --- ЛОГИКА WEBSOCKETS ---
class ConnectionManager:
    def __init__(self):
        self.active_connections: List[WebSocket] = []

    async def connect(self, websocket: WebSocket):
        await websocket.accept()
        self.active_connections.append(websocket)

    def disconnect(self, websocket: WebSocket):
        if websocket in self.active_connections:
            self.active_connections.remove(websocket)

    async def broadcast(self, data: dict):
        for connection in self.active_connections:
            try:
                await connection.send_text(json.dumps(data))
            except Exception:
                # Если соединение оборвалось, но не удалилось
                continue

manager = ConnectionManager()

# 4. Инициализация FastAPI
app = FastAPI(title="Todo App API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()

# --- ROOT ENDPOINT ---
@app.get("/")
def read_root():
    return {
        "message": "Todo App API",
        "docs": "http://127.0.0.1:8000/docs",
        "endpoints": {
            "tasks": "GET /tasks - List all tasks",
            "create_task": "POST /tasks - Create a new task",
            "get_task": "GET /tasks/{task_id} - Get a specific task",
            "update_task": "PUT /tasks/{task_id} - Update a task",
            "delete_task": "DELETE /tasks/{task_id} - Delete a task",
            "websocket": "WebSocket /ws - Real-time updates"
        }
    }

@app.get("/health")
def health_check():
    return {"status": "healthy"}

# --- WEBSOCKET ENDPOINT ---
@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    await manager.connect(websocket)
    try:
        while True:
            await websocket.receive_text()
    except WebSocketDisconnect:
        manager.disconnect(websocket)

# --- ЭНДПОИНТЫ ДЛЯ ЗАДАЧ ---

@app.get("/tasks", response_model=List[TaskResponse])
def read_tasks(db: Session = Depends(get_db)):
    return db.query(Task).all()

@app.post("/tasks", response_model=TaskResponse)
async def create_task(task: TaskCreate, db: Session = Depends(get_db)):
    db_task = Task(title=task.title, description=task.description)
    db.add(db_task)
    db.commit()
    db.refresh(db_task)
    await manager.broadcast({"event": "update", "message": "Добавлена новая задача"})
    return db_task

@app.get("/tasks/{task_id}", response_model=TaskResponse)
def read_task(task_id: int, db: Session = Depends(get_db)):
    db_task = db.query(Task).filter(Task.id == task_id).first()
    if db_task is None:
        raise HTTPException(status_code=404, detail="Task not found")
    return db_task

@app.put("/tasks/{task_id}", response_model=TaskResponse)
async def update_task(task_id: int, updated_data: TaskCreate, db: Session = Depends(get_db)):
    db_task = db.query(Task).filter(Task.id == task_id).first()
    if db_task is None:
        raise HTTPException(status_code=404, detail="Task not found")
    db_task.title = updated_data.title
    db_task.description = updated_data.description
    db.commit()
    db.refresh(db_task)
    await manager.broadcast({"event": "update", "message": f"Задача {task_id} обновлена"})
    return db_task

@app.delete("/tasks/{task_id}")
async def delete_task(task_id: int, db: Session = Depends(get_db)):
    db_task = db.query(Task).filter(Task.id == task_id).first()
    if db_task is None:
        raise HTTPException(status_code=404, detail="Task not found")
    db.delete(db_task)
    db.commit()
    await manager.broadcast({"event": "update", "message": f"Задача {task_id} удалена"})
    return {"message": "Task deleted successfully"}

# --- ПОЧТОВЫЕ ФУНКЦИИ ---

@app.post("/tasks/{task_id}/email")
def send_email_mailhog(task_id: int, db: Session = Depends(get_db)):
    task = db.query(Task).filter(Task.id == task_id).first()
    if not task: 
        raise HTTPException(status_code=404, detail="Задача не найдена")

    msg = MIMEText(f"ID: {task.id}\nЗаголовок: {task.title}\nОписание: {task.description}")
    msg['Subject'] = f"Лабораторная: {task.title}"
    msg['From'] = "todo-app@local.test"
    msg['To'] = "student@example.com"

    try:
        with smtplib.SMTP(SMTP_SERVER, SMTP_PORT) as server:
            server.send_message(msg)
        return {"message": "Письмо отправлено!"}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Ошибка отправки: {str(e)}")

@app.get("/mail/imap")
def check_mail_imap():
    return {"protocol": "IMAP", "info": "Чтение заголовков без удаления"}

@app.get("/mail/pop3")
def check_mail_pop3():
    return {"protocol": "POP3", "info": "Загрузка писем на устройство"}

# 5. Точка входа для запуска
if __name__ == "__main__":
    uvicorn.run("main:app", host="127.0.0.1", port=8000, reload=True)