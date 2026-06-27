# Library Management System — Design Spec
**Date:** 2026-06-26
**Stack:** .NET 10, ASP.NET Core Web API, Entity Framework Core (Code-First), SQL Server

---

## 1. Overview

A RESTful Library Management System exposing CRUD operations for books, members, system users, and borrowing transactions. Authentication uses HTTP Basic Auth (username + password in every request header). Role-based access control (RBAC) enforces permissions at the controller level. All responses use a standard `ApiResponse<T>` envelope with descriptive human-readable messages.

---

## 2. Solution Structure

```
LibraryManagementSystem.sln
├── src/
│   ├── LibraryMS.Domain/           ← entities, enums, repository interfaces (no external deps)
│   ├── LibraryMS.Application/      ← service interfaces + implementations, DTOs, ViewModels, ServiceResult
│   ├── LibraryMS.Infrastructure/   ← EF Core DbContext, Fluent API config, repository impls, migrations
│   └── LibraryMS.API/              ← controllers, BasicAuth middleware, Program.cs, DI wiring
└── docs/
    ├── ERD.md
    └── sql/
        ├── schema.sql              ← generated from EF migrations
        └── seed.sql                ← sample data
```

**Dependency rule:** `API → Infrastructure → Application → Domain`. Domain has zero external dependencies.

| Project | Key Contents |
|---|---|
| `LibraryMS.Domain` | `BaseEntity`, all entities, `BookStatus`, `UserRole` enums, `IRepository<T>` and specific repo interfaces |
| `LibraryMS.Application` | Service interfaces + implementations, DTOs (requests), ViewModels (responses), `ApiResponse<T>`, `ServiceResult<T>` |
| `LibraryMS.Infrastructure` | `LibraryDbContext`, entity configurations, generic + specific repository implementations, migrations |
| `LibraryMS.API` | Controllers, `BasicAuthMiddleware`, `Program.cs`, `appsettings.json` |

---

## 3. Technology Stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core 10 Web API |
| ORM | Entity Framework Core (Code-First) |
| Database | SQL Server |
| Authentication | HTTP Basic Auth (custom middleware) |
| Password hashing | `BCrypt.Net-Next` NuGet package |
| API documentation | Built-in OpenAPI (`/openapi`) |
| Cover images | File path stored in DB; files saved to `wwwroot/covers/` |
| Activity logging | `ActivityLogs` database table |

---

## 4. Domain — BaseEntity & Entities

### BaseEntity
All auditable entities inherit from `BaseEntity`:

```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
}
```

A global EF Core query filter `WHERE IsDeleted = 0` is applied to all `BaseEntity` descendants — soft-deleted records are invisible by default.

### Entities

**Inherits BaseEntity:** `Book`, `Author`, `Publisher`, `Category`, `Member`, `SystemUser`, `BorrowingTransaction`

**Does NOT inherit BaseEntity:** `BookAuthor` (join table), `BookCategory` (join table), `ActivityLog` (append-only, never updated or deleted)

```
Book
  Id, Title, ISBN, Edition, Language, PublicationYear
  Summary, CoverImagePath, Status (Available/Borrowed)
  PublisherId (FK → Publisher)
  + BaseEntity fields

Author
  Id, FirstName, LastName, Bio
  + BaseEntity fields

Publisher
  Id, Name, Address, Website
  + BaseEntity fields

Category
  Id, Name, ParentCategoryId (nullable FK → self)
  + BaseEntity fields
  [Max two levels: ParentCategoryId=null means top-level]

BookAuthor     (BookId, AuthorId)           ← composite PK
BookCategory   (BookId, CategoryId)         ← composite PK

Member
  Id, FirstName, LastName, Email, Phone
  MembershipDate, IsActive
  + BaseEntity fields

SystemUser
  Id, Username, PasswordHash, Role, FullName, Email, IsActive
  + BaseEntity fields

BorrowingTransaction
  Id, BookId (FK), MemberId (FK)
  BorrowedByUserId (FK → SystemUser)
  ReturnedByUserId (FK → SystemUser, nullable)
  BorrowDate, DueDate, ReturnDate (nullable)
  Status (Active/Returned)
  + BaseEntity fields

ActivityLog
  Id, UserId, Action, EntityType, EntityId (nullable string)
  Timestamp, Details (JSON string)
```

### Enums

```csharp
public enum BookStatus   { Available, Borrowed }
public enum UserRole     { Administrator, Librarian, Staff }
public enum BorrowStatus { Active, Returned }
```

---

## 5. Database Schema

### Key design decisions
- `Categories.ParentCategoryId` nullable → null = top-level parent; non-null = child. Application layer blocks 3rd level.
- `Book.Status` reflects real-time availability; updated atomically on borrow/return.
- `BorrowingTransaction` stores both `BorrowedByUserId` and `ReturnedByUserId` for full staff audit trail.
- `ActivityLog.EntityId` is `string?` (not a FK) so it can reference any table without constraint coupling.
- `PasswordHash` stores BCrypt hash — plain passwords are never persisted or logged.

---

## 6. Application Layer

### ServiceResult<T>
All service methods return `ServiceResult<T>` — no business logic in controllers:

```csharp
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }
}
```

### ApiResponse<T> (ViewModel)
Every controller action returns:

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}
```

### DTOs (Request objects) — in `LibraryMS.Application/DTOs/`
One `Create` DTO and one `Update` DTO per entity (e.g., `CreateBookDto`, `UpdateBookDto`).

### ViewModels (Response objects) — in `LibraryMS.Application/ViewModels/`
One ViewModel per entity for GET responses (e.g., `BookViewModel`, `MemberViewModel`). ViewModels flatten related data (e.g., `BookViewModel` includes `AuthorNames[]`, `CategoryNames[]`, `PublisherName`).

### Services
| Service | Responsibilities |
|---|---|
| `BookService` | CRUD, cover upload, search (name/author/category), filter by status |
| `AuthorService` | CRUD |
| `PublisherService` | CRUD |
| `CategoryService` | CRUD, enforce two-level hierarchy |
| `MemberService` | CRUD |
| `UserService` | CRUD, password hashing |
| `BorrowingService` | Borrow, return, transaction history |
| `ActivityLogService` | Append log entries; called by every service after write operations |

---

## 7. Authentication & Authorization

### HTTP Basic Auth Flow
1. Client sends `Authorization: Basic <base64(username:password)>` on every request.
2. `BasicAuthMiddleware` decodes the header, looks up `SystemUsers` by username.
3. Verifies password using `BCrypt.Verify()`.
4. On success: attaches `ClaimsPrincipal` with role claim to `HttpContext.User`.
5. On failure: returns `401` with message `"Invalid username or password. Please check your credentials and try again."`

### RBAC Matrix

| Endpoint Group | Administrator | Librarian | Staff |
|---|---|---|---|
| Books — Write (create/update) | ✅ | ✅ | ❌ |
| Books — Delete | ✅ | ❌ | ❌ |
| Books — Read / Search | ✅ | ✅ | ✅ |
| Authors / Publishers / Categories — Write | ✅ | ✅ | ❌ |
| Authors / Publishers / Categories — Read | ✅ | ✅ | ✅ |
| Members — Write | ✅ | ✅ | ❌ |
| Members — Read | ✅ | ✅ | ❌ |
| System Users — All | ✅ | ❌ | ❌ |
| Borrow / Return | ✅ | ✅ | ✅ |
| Borrowing Transactions — Read | ✅ | ✅ | ❌ |
| Activity Logs — Read | ✅ | ✅ | ❌ |

Controllers use `[Authorize(Roles = "Administrator,Librarian")]` etc. via standard ASP.NET Core policy enforcement.

---

## 8. API Endpoints

### Books
```
GET    /api/books                                  → all books
GET    /api/books/{id}                             → book detail
POST   /api/books                                  → create book (Librarian/Admin)
PUT    /api/books/{id}                             → update book (Librarian/Admin)
DELETE /api/books/{id}                             → soft delete (Admin)
POST   /api/books/{id}/cover                       → upload cover image (Librarian/Admin)
GET    /api/books/search?name=&author=&category=   → search [BONUS]
GET    /api/books/by-status?status=Available       → filter by status [BONUS]
```

### Authors
```
GET    /api/authors
GET    /api/authors/{id}
POST   /api/authors        (Librarian/Admin)
PUT    /api/authors/{id}   (Librarian/Admin)
DELETE /api/authors/{id}   (Admin)
```

### Publishers
```
GET    /api/publishers
GET    /api/publishers/{id}
POST   /api/publishers        (Librarian/Admin)
PUT    /api/publishers/{id}   (Librarian/Admin)
DELETE /api/publishers/{id}   (Admin)
```

### Categories
```
GET    /api/categories
GET    /api/categories/{id}
POST   /api/categories        (Librarian/Admin)
PUT    /api/categories/{id}   (Librarian/Admin)
DELETE /api/categories/{id}   (Admin)
```

### Members
```
GET    /api/members            (Librarian/Admin)
GET    /api/members/{id}       (Librarian/Admin)
POST   /api/members            (Librarian/Admin)
PUT    /api/members/{id}       (Librarian/Admin)
DELETE /api/members/{id}       (Admin)
```

### System Users
```
GET    /api/users              (Admin)
GET    /api/users/{id}         (Admin)
POST   /api/users              (Admin)
PUT    /api/users/{id}         (Admin)
DELETE /api/users/{id}         (Admin)
```

### Borrowing
```
POST   /api/borrowing/borrow                      → borrow a book (all roles)
POST   /api/borrowing/return/{transactionId}      → return a book (all roles)
GET    /api/borrowing/transactions                → all transactions (Librarian/Admin)
GET    /api/borrowing/member/{memberId}           → member history (Librarian/Admin)
```

### Activity Logs
```
GET    /api/logs                   (Librarian/Admin)
GET    /api/logs/user/{userId}     (Admin)
```

---

## 9. Business Rules

### Borrowing
- Book with `Status = Borrowed` cannot be borrowed: `"Book '{Title}' (ISBN: {ISBN}) is currently checked out and cannot be borrowed again until it is returned."`
- Member with 5 active borrows is blocked: `"Member '{FullName}' has reached the maximum of 5 active borrows. A book must be returned before a new one can be borrowed."`
- Default loan period: 14 days (`DueDate = BorrowDate + 14`)
- Returning already-returned book: `"Transaction {id} has already been closed. This book was returned on {ReturnDate}."`
- On borrow: `Book.Status → Borrowed`, transaction created, activity logged
- On return: `Book.Status → Available`, `ReturnDate` set, transaction `Status → Returned`, activity logged

### Categories
- A child category cannot be set as parent: `"Category '{Name}' is already a child category and cannot be used as a parent. Only two levels of hierarchy are allowed."`
- Parent deletion blocked if children exist: `"Cannot delete category '{Name}' because it has {count} child categories. Remove or reassign them first."`

### Books
- Duplicate ISBN blocked: `"A book with ISBN '{ISBN}' already exists in the system."`

### System Users
- Soft-deleted user cannot authenticate: `"Your account has been deactivated. Please contact an administrator."`

### Descriptive Error Messages (all scenarios)
| Scenario | HTTP | Message |
|---|---|---|
| Book already borrowed | 400 | `"Book '{Title}' (ISBN: {ISBN}) is currently checked out..."` |
| Member borrow limit | 400 | `"Member '{FullName}' has reached the maximum of 5 active borrows..."` |
| Not found | 404 | `"No {entity} found with ID {id}. It may have been deleted or never existed."` |
| Duplicate ISBN | 409 | `"A book with ISBN '{ISBN}' already exists in the system."` |
| Category depth | 400 | `"Category '{Name}' is already a child category..."` |
| Invalid credentials | 401 | `"Invalid username or password. Please check your credentials and try again."` |
| Insufficient role | 403 | `"Access denied. This action requires the '{RequiredRole}' role."` |
| Soft-deleted record | 404 | `"The requested resource no longer exists or has been removed from the system."` |
| Return already returned | 400 | `"Transaction {id} has already been closed. This book was returned on {ReturnDate}."` |
| Deactivated account | 401 | `"Your account has been deactivated. Please contact an administrator."` |

---

## 10. Activity Logging

Logged after every successful write operation by the relevant service:

```
ActivityLog
  UserId       → authenticated user's ID
  Action       → e.g. "CreateBook", "BorrowBook", "ReturnBook", "DeleteMember"
  EntityType   → e.g. "Book", "Member", "SystemUser"
  EntityId     → string representation of affected record ID (nullable)
  Timestamp    → UTC now
  Details      → JSON snippet of key fields changed
```

Logging is synchronous within the same request — no background job. `ActivityLogService` is injected into every other service.

---

## 11. Cover Images

- `POST /api/books/{id}/cover` accepts `multipart/form-data` with an image file
- File saved to `wwwroot/covers/{bookId}_{filename}`
- `Book.CoverImagePath` updated with the relative URL (e.g., `/covers/1_cover.jpg`)
- No image processing — raw file storage only

---

## 12. Deliverables Checklist

- [ ] 4-project Clean Architecture solution
- [ ] EF Core migrations generating the full schema
- [ ] `docs/sql/schema.sql` — scripted from migrations
- [ ] `docs/sql/seed.sql` — sample data (3 users per role, 10+ books, 5+ members, borrow transactions)
- [ ] `docs/ERD.md` — entity relationship diagram (text/ASCII)
- [ ] `README.md` — design choices, setup instructions, how to run
- [ ] Postman collection — all endpoints with Basic Auth pre-configured [BONUS]
- [ ] Search endpoint — by name, author, category [BONUS]
- [ ] Books by status endpoint [BONUS]
