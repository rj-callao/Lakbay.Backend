# Piloopinas Backend

ASP.NET Core 10 Web API using Clean Architecture, Entity Framework Core, and SQL Server LocalDB.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- SQL Server LocalDB (included with Visual Studio) or SQL Server Express
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet) — install globally:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## Getting Started

### 1. Clone the repository

```bash
git clone <repo-url>
cd Piloopinas/Piloopinas.Backend
```

### 2. Create the database migration

```bash
dotnet ef migrations add InitialCreate --project src/Piloopinas.Infrastructure --startup-project src/Piloopinas.Api
```

### 3. Run the application

```bash
dotnet run --project src/Piloopinas.Api
```

The API will start on **http://localhost:5107**. On first run it will automatically:
- Apply pending migrations (create the database)
- Seed sample data (admin user, riders, events, motorcycles, badges, registrations)

### 4. Verify it's working

Open Swagger UI: **http://localhost:5107/swagger**

Or test login via terminal:
```bash
curl -X POST http://localhost:5107/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"admin@piloopinas.com\",\"password\":\"Admin@123\"}"
```

## Default Credentials

| User | Email | Password | Role |
|------|-------|----------|------|
| Admin | admin@piloopinas.com | Admin@123 | Admin |
| Juan Dela Cruz | juan@example.com | Rider@123 | Rider |
| Maria Santos | maria@example.com | Rider@123 | Rider |
| Pedro Reyes | pedro@example.com | Rider@123 | Rider |

## Project Structure

```
Piloopinas.Backend/
├── src/
│   ├── Piloopinas.Api/              # Controllers, middleware, DI config
│   ├── Piloopinas.Application/      # DTOs, interfaces, contracts
│   ├── Piloopinas.Domain/           # Entities, constants
│   └── Piloopinas.Infrastructure/   # EF Core, service implementations
└── Piloopinas.slnx                  # Solution file
```

## Useful Commands

```bash
# Build
dotnet build

# Run
dotnet run --project src/Piloopinas.Api

# Reset database
dotnet ef database drop --project src/Piloopinas.Infrastructure --startup-project src/Piloopinas.Api --force
dotnet ef migrations remove --project src/Piloopinas.Infrastructure --startup-project src/Piloopinas.Api --force
dotnet ef migrations add InitialCreate --project src/Piloopinas.Infrastructure --startup-project src/Piloopinas.Api
```

## Setting Up a Multi-Project Workspace (VS Code)

To work on both **Backend** and **Frontend** in a single VS Code window:

1. Open VS Code
2. Go to **File → Add Folder to Workspace...**
3. Add the `Piloopinas.Backend` folder
4. Go to **File → Add Folder to Workspace...** again
5. Add the `Piloopinas.Frontend` folder
6. Go to **File → Save Workspace As...** and save as `Piloopinas.code-workspace` in the parent `Piloopinas/` folder

Or create a `Piloopinas.code-workspace` file manually in the parent directory:

```json
{
  "folders": [
    { "path": "Piloopinas.Backend" },
    { "path": "Piloopinas.Frontend" }
  ],
  "settings": {}
}
```

Then open it with **File → Open Workspace from File...**