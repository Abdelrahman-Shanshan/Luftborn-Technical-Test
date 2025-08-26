# Company Todo API (ASP.NET Core + SQL + Optional SSO)

A clean, minimal-yet-scalable starter that satisfies your requirements:

- **Backend:** .NET 8 (works for ".NET Core 3.1+" requirement, but uses latest LTS).
- **Database:** SQL Server via EF Core.
- **CRUD:** `TodoItem` with DTOs, service layer, repository & unit-of-work.
- **SSO (Optional):** Plug-in JWT/OpenID Connect (e.g., **ADFS**) with `AddJwtBearer`.
- **Scalability & Readability:** Single-responsibility classes, meaningful names, minimal external libraries.
- **Swagger:** API docs + JWT Bearer support.

---

## 1) Create & Run

### Prereqs
- .NET SDK 8.x
- SQL Server (Developer/Express/localdb)
- (Optional) ADFS / external IdP that can issue JWT (OpenID Connect)

### Restore & Run
```bash
cd src/Company.Todo.Api
dotnet restore
dotnet ef database update   # creates CompanyTodoDb using migrations once you add one (see below)
dotnet run
```

The API will start on `https://localhost:5001` and open **Swagger** at `/swagger`.

> **Note**: This template does not ship a migration file to keep it clean. Create one after you review the model:
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 2) App Structure

```
Company.Todo.Api
├─ Controllers
│  └─ TodoItemsController.cs        # HTTP endpoints
├─ Data
│  └─ AppDbContext.cs               # EF Core DbContext
├─ Dtos
│  ├─ CreateTodoItemDto.cs
│  ├─ TodoItemDto.cs
│  └─ UpdateTodoItemDto.cs
├─ Models
│  └─ TodoItem.cs                   # Entity
├─ Repositories
│  ├─ IGenericRepository.cs
│  ├─ ITodoRepository.cs
│  ├─ GenericRepository.cs
│  ├─ TodoRepository.cs
│  ├─ IUnitOfWork.cs
│  └─ UnitOfWork.cs
├─ Services
│  ├─ ITodoService.cs
│  └─ TodoService.cs
├─ Program.cs                        # Composition root (DI/Middleware/Auth/Swagger)
├─ appsettings.json                  # Connection string & (optional) Auth
└─ Properties/launchSettings.json
```

Design choices:
- **Single Purpose:** Controllers only orchestrate; business logic sits in `Services`; data access in `Repositories`.
- **Minimal Dependencies:** No AutoMapper or heavy frameworks; mapping is explicit and readable.
- **Extensible:** Add modules by repeating this pattern per aggregate (entity).

---

## 3) CRUD Endpoints (Examples)

- `GET /api/todoitems?page=1&pageSize=10&search=report`
- `GET /api/todoitems/5`
- `POST /api/todoitems`
  ```json
  { "title": "Write docs", "description": "Keep it clear" }
  ```
- `PUT /api/todoitems/5`
  ```json
  { "title": "Write better docs", "description": "Short & clear", "isCompleted": true }
  ```
- `DELETE /api/todoitems/5`

---

## 4) Configure SQL Server

Edit `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CompanyTodoDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Use a SQL login if preferred:
```json
"DefaultConnection": "Server=localhost;Database=CompanyTodoDb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
```

---

## 5) Optional SSO (ADFS or any OpenID Connect)

Set these in `appsettings.json`:
```json
"Authentication": {
  "Authority": "https://adfs.yourdomain.com/adfs",
  "Audience": "api://company.todo"
}
```

Then, in **ADFS**:
1. Create an **Application Group** that supports **OpenID Connect**.
2. Configure a **Web API** and set **Identifier** to `api://company.todo` (your Audience).
3. Enable JWT issuance (ADFS 2016+ supports OpenID Connect).
4. Issue standard claims (sub, name, email, roles as needed).

Finally, protect any endpoint with `[Authorize]` (see `/api/todoitems/me`).

**Testing JWT in Swagger:**
1. Click *Authorize* in Swagger UI.
2. Enter `Bearer {your_jwt}` obtained from your IdP (client app or Postman).

---

## 6) Production Hardening Checklist

- Replace `AllowAll` CORS with explicit origins.
- Add **Serilog** (optional) for structured logging.
- Add **FluentValidation** or minimal custom validators (optional).
- Use **retry policies** for SQL (e.g., `EnableRetryOnFailure`).
- Write **unit tests** for Services (mock `IUnitOfWork`).
- Add **health checks** and **readiness/liveness** probes.
- Containerize with a **Dockerfile** and use environment variables for secrets.

---

## 7) Clean Code Notes

- Meaningful names, small classes, single responsibility.
- Comments only where non-obvious (e.g., ADFS notes).
- Avoid premature abstraction; keep patterns pragmatic.

---

## 8) Next Steps (if you want to extend)

- Add **Users** / **Roles** tables and policy-based authorization.
- Add **Soft delete** and **auditing** (CreatedBy, ModifiedBy).
- Add **Pagination metadata** in headers (X-Total-Count).

---

Happy building! 🚀
