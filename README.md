# CleanTask API

A production-ready **Task Management REST API** built with **ASP.NET Core 8** following **Clean Architecture** principles. Designed as a portfolio demonstration of enterprise-grade .NET backend development.

---

## Architecture Overview

```
CleanTaskAPI/
├── src/
│   ├── CleanTask.Domain/          # Enterprise business rules
│   │   ├── Entities/              # TaskItem, User
│   │   ├── Enums/                 # TaskStatus, TaskPriority
│   │   ├── Interfaces/            # IRepository, IUnitOfWork
│   │   └── Common/                # BaseEntity (audit fields)
│   │
│   ├── CleanTask.Application/     # Application business rules
│   │   ├── Features/
│   │   │   ├── Tasks/
│   │   │   │   ├── Commands/      # CreateTask, DeleteTask, AssignTask, UpdateTaskStatus
│   │   │   │   └── Queries/       # GetAllTasks, GetTaskById, GetTasksByUser
│   │   │   └── Users/
│   │   │       ├── Commands/      # CreateUser, Login
│   │   │       └── Queries/       # GetAllUsers
│   │   ├── Common/
│   │   │   ├── Behaviours/        # ValidationBehaviour (MediatR pipeline)
│   │   │   ├── Exceptions/        # NotFoundException, ValidationException, etc.
│   │   │   └── Interfaces/        # IJwtService, IPasswordService
│   │   └── DTOs/                  # TaskDto, UserDto, AuthResponseDto
│   │
│   ├── CleanTask.Infrastructure/  # Frameworks & drivers
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Configurations/    # EF Core entity configurations
│   │   ├── Repositories/          # Repository<T>, TaskRepository, UserRepository, UnitOfWork
│   │   └── Services/              # JwtService, PasswordService (BCrypt)
│   │
│   └── CleanTask.API/             # Delivery mechanism
│       ├── Controllers/           # TasksController, UsersController
│       ├── Middleware/            # Global exception handling
│       └── Extensions/            # JWT auth, Swagger setup
│
└── tests/
    ├── CleanTask.Application.Tests/
    └── CleanTask.API.Tests/
```

### Dependency Rule
```
API → Application → Domain
Infrastructure → Application → Domain
```
Domain and Application have zero dependencies on frameworks or infrastructure.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 / PostgreSQL 16 (switchable) |
| CQRS | MediatR 12 |
| Validation | FluentValidation 11 (pipeline behaviour) |
| Authentication | JWT Bearer tokens |
| Password Hashing | BCrypt.Net |
| API Docs | Swagger / OpenAPI |
| Containerisation | Docker + docker-compose |

---

## Key Design Decisions

### Clean Architecture Layers
Each layer has a single responsibility. The Domain layer has no dependencies — it contains only pure C# classes and interfaces. The Application layer orchestrates business logic using CQRS with MediatR. Infrastructure implements the interfaces defined in the Application layer. The API layer is purely a delivery mechanism.

### CQRS with MediatR
Every operation is a Command (write) or Query (read). This separates read and write concerns cleanly and makes the codebase easy to navigate — every feature lives in its own folder with its Command/Query, Validator, and Handler co-located.

### Validation Pipeline Behaviour
FluentValidation runs automatically for every Command and Query via a MediatR pipeline behaviour. Controllers never contain validation logic. Invalid requests are rejected before they reach the handler.

### Dual Database Support
Switch between SQL Server and PostgreSQL by changing one line in `appsettings.json`:
```json
"DatabaseProvider": "SqlServer"   // or "PostgreSQL"
```

### Soft Delete
Deleted tasks are never physically removed. EF Core global query filters ensure deleted records are invisible to all queries automatically — no `WHERE IsDeleted = false` needed in application code.

### Global Exception Handling
A middleware layer catches all unhandled exceptions and maps them to consistent, well-formed JSON error responses. Controllers never contain try-catch blocks.

---

## Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- .NET 8 SDK (for local development without Docker)

### Run with Docker (recommended)

```bash
git clone https://github.com/yourusername/CleanTaskAPI.git
cd CleanTaskAPI
docker-compose up --build
```

The API starts at **http://localhost:5000**
Swagger UI is available at **http://localhost:5000** (root)

### Run locally without Docker

```bash
# 1. Update connection string in appsettings.Development.json

# 2. Apply migrations
cd src/CleanTask.API
dotnet ef database update --project ../CleanTask.Infrastructure

# 3. Run
dotnet run
```

---

## API Endpoints

### Authentication
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/users/register` | None | Register a new user |
| POST | `/api/users/login` | None | Login and receive JWT token |

### Tasks
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/tasks` | Required | Get all tasks |
| GET | `/api/tasks/{id}` | Required | Get task by ID |
| POST | `/api/tasks` | Required | Create a new task |
| PATCH | `/api/tasks/{id}/status` | Required | Update task status |
| PATCH | `/api/tasks/{id}/assign` | Required | Assign task to a user |
| DELETE | `/api/tasks/{id}` | Required | Soft delete a task |

### Users
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/users` | Required | Get all users |

### Health
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/health` | None | Health check |

---

## Example Usage

### 1. Register a user
```http
POST /api/users/register
Content-Type: application/json

{
  "firstName": "Anuradha",
  "lastName": "Madhushani",
  "email": "anu@example.com",
  "password": "SecurePass123"
}
```

### 2. Login
```http
POST /api/users/login
Content-Type: application/json

{
  "email": "anu@example.com",
  "password": "SecurePass123"
}
```
Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "email": "anu@example.com",
  "fullName": "Anuradha Madhushani",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "expiresAt": "2025-01-02T10:00:00Z"
}
```

### 3. Create a task (use the token from step 2)
```http
POST /api/tasks
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "title": "Implement authentication module",
  "description": "Build JWT-based auth with refresh tokens",
  "priority": "High",
  "dueDate": "2025-03-01T00:00:00Z",
  "assignedToUserId": null
}
```

### 4. Update task status
```http
PATCH /api/tasks/{id}/status
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
Content-Type: application/json

{
  "status": "InProgress"
}
```

---

## Switching Databases

### Use PostgreSQL instead of SQL Server

1. Change in `appsettings.json`:
```json
"DatabaseProvider": "PostgreSQL"
```

2. Regenerate migrations:
```bash
dotnet ef migrations add InitialCreate \
  --project src/CleanTask.Infrastructure \
  --startup-project src/CleanTask.API \
  -- --provider PostgreSQL
```

3. In `docker-compose.yml` update the `api` service environment:
```yaml
- DatabaseProvider=PostgreSQL
```

---

## Author

**Anuradha Madhushani**
Senior Full-Stack Software Engineer · 8+ years experience
.NET Core · Azure · Clean Architecture · React · TypeScript

📍 Schmalkalden, Germany
🎓 MSc Applied Computer Science — Hochschule Schmalkalden
💼 Available for senior contract work across Europe

[LinkedIn](https://linkedin.com/in/yourprofile) · [GitHub](https://github.com/yourusername)
