# Library Management System

A RESTful Web API built with **.NET 10**, **ASP.NET Core**, **Entity Framework Core**, and **SQL Server**.

---

## Design Choices

### Architecture — Clean Architecture (4 Projects)

| Project | Purpose |
|---|---|
| `LibraryMS.Domain` | Entities, enums. No external dependencies. |
| `LibraryMS.Application` | Service interfaces + implementations, DTOs (requests), ViewModels (responses), `ServiceResult<T>`, `ApiResponse<T>`. |
| `LibraryMS.Infrastructure` | EF Core `DbContext`, Fluent API config, migrations. |
| `LibraryMS.API` | Controllers, `BasicAuthMiddleware`, `Program.cs`. |

Dependency rule: `API → Infrastructure → Application → Domain`.

### Authentication — HTTP Basic Auth
Every request must include `Authorization: Basic <base64(username:password)>`.  
A custom `BasicAuthMiddleware` decodes the header, looks up the user, verifies the BCrypt hash, and attaches a `ClaimsPrincipal` to `HttpContext.User`. No JWT, no sessions.

### Authorization — Role-Based
Three roles: `Administrator`, `Librarian`, `Staff`.  
Controllers use `[Authorize(Roles = "...")]` attributes. See the RBAC matrix in the spec.

### Password Storage
Passwords are hashed with **BCrypt** (`BCrypt.Net-Next`) using work factor 11. Plain passwords are never stored or logged.

### Soft Deletes
All main entities inherit `BaseEntity` which has `IsDeleted`. EF Core global query filters `WHERE IsDeleted = 0` make soft-deleted records invisible automatically.

### Descriptive Error Messages
Every business rule violation returns a specific, human-readable message explaining *why* the request failed (e.g., which book is checked out, which member hit the limit).

### Activity Logging
Every successful write operation logs to the `ActivityLogs` SQL table synchronously within the same request. No background jobs.

---

## Prerequisites

- .NET 10 SDK
- SQL Server (local or remote)

---

## Setup & Run

### 1. Configure the connection string

Edit `src/LibraryMS.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=LibraryMS;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Run the API

```bash
cd src/LibraryMS.API
dotnet run
```

The app automatically runs EF Core migrations on startup (in Development environment). The database and all tables are created automatically.

### 3. Load sample data (optional)

After the schema is created, run:

```bash
sqlcmd -S localhost -d LibraryMS -i docs/sql/seed.sql
```

Or execute `docs/sql/seed.sql` in SQL Server Management Studio.

### 4. Test with Basic Auth

All endpoints require an `Authorization` header. Example using curl:

```bash
# admin1 / Admin@123 → base64: YWRtaW4xOkFkbWluQDEyMw==
curl -H "Authorization: Basic YWRtaW4xOkFkbWluQDEyMw==" http://localhost:5000/api/books
```

---

## API Endpoints

| Method | Route | Roles | Description |
|---|---|---|---|
| GET | `/api/books` | All | List all books |
| GET | `/api/books/{id}` | All | Book detail |
| POST | `/api/books` | Admin, Librarian | Create book |
| PUT | `/api/books/{id}` | Admin, Librarian | Update book |
| DELETE | `/api/books/{id}` | Admin | Soft-delete book |
| POST | `/api/books/{id}/cover` | Admin, Librarian | Upload cover image |
| GET | `/api/books/search?name=&author=&category=` | All | Search books |
| GET | `/api/books/by-status?status=Available` | All | Filter by status |
| GET | `/api/authors` | All | List authors |
| POST | `/api/authors` | Admin, Librarian | Create author |
| PUT | `/api/authors/{id}` | Admin, Librarian | Update author |
| DELETE | `/api/authors/{id}` | Admin | Delete author |
| GET | `/api/publishers` | All | List publishers |
| POST | `/api/publishers` | Admin, Librarian | Create publisher |
| GET | `/api/categories` | All | List categories (tree) |
| POST | `/api/categories` | Admin, Librarian | Create category |
| GET | `/api/members` | Admin, Librarian | List members |
| POST | `/api/members` | Admin, Librarian | Create member |
| GET | `/api/users` | Admin | List system users |
| POST | `/api/users` | Admin | Create system user |
| POST | `/api/borrowing/borrow` | All | Borrow a book |
| POST | `/api/borrowing/return/{id}` | All | Return a book |
| GET | `/api/borrowing/transactions` | Admin, Librarian | All transactions |
| GET | `/api/borrowing/member/{id}` | Admin, Librarian | Member history |
| GET | `/api/logs` | Admin, Librarian | Activity logs |
| GET | `/api/logs/user/{userId}` | Admin | Logs by user |

---

## Standard Response Format

All endpoints return:

```json
{
  "success": true,
  "message": "Book created successfully.",
  "data": { ... },
  "errors": null
}
```

Error example:

```json
{
  "success": false,
  "message": "Book 'Dune' (ISBN: 978-0441013593) is currently checked out and cannot be borrowed again until it is returned.",
  "data": null,
  "errors": null
}
```

---

## Project Structure

```
src/
├── LibraryMS.Domain/           ← Entities, Enums
├── LibraryMS.Application/      ← Services, DTOs, ViewModels
├── LibraryMS.Infrastructure/   ← DbContext, EF config, migrations
└── LibraryMS.API/              ← Controllers, Middleware, Program.cs
```
