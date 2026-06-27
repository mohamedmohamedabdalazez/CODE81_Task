# Library Management System — Entity Relationship Diagram

## Entities & Relationships

```
┌─────────────────────────────────────────────────────────────────────────────────────────────┐
│ LEGEND:  PK = Primary Key   FK = Foreign Key   * = nullable   [soft-delete] = IsDeleted    │
└─────────────────────────────────────────────────────────────────────────────────────────────┘

┌──────────────────┐         ┌──────────────────────┐         ┌──────────────────┐
│   Publisher      │         │        Book           │         │     Author       │
│──────────────────│         │──────────────────────│         │──────────────────│
│ PK Id            │ 1     * │ PK Id                │ *     * │ PK Id            │
│    Name          ├────────►│    Title             │         │    FirstName     │
│    Address *     │         │    ISBN (unique)      │         │    LastName      │
│    Website *     │         │    Edition *          │         │    Bio *         │
│ [soft-delete]    │         │    Language *         │         │ [soft-delete]    │
│                  │         │    PublicationYear *  │         └────────┬─────────┘
└──────────────────┘         │    Summary *          │                  │
                             │    CoverImagePath *   │                  │ *
                             │    Status             │         ┌────────┴──────────┐
                             │ FK PublisherId        │         │    BookAuthor      │
                             │ [soft-delete]         │ 1     * │───────────────────│
                             └──────────┬────────────┘         │ PK,FK BookId      │
                                        │                       │ PK,FK AuthorId    │
                                        │ 1                     └────────────────────┘
                                        │
                             ┌──────────┴────────────┐
                             │    BookCategory        │         ┌──────────────────┐
                             │───────────────────────│         │     Category     │
                             │ PK,FK BookId           │ *     1 │──────────────────│
                             │ PK,FK CategoryId      ├────────►│ PK Id            │
                             └────────────────────────┘         │    Name          │
                                                                 │ FK ParentCat* ──┐│
                                                                 │ [soft-delete]   ││
                                                                 └─────────────────┘│
                                                                       ▲            │
                                                                       └────────────┘
                                                                     (self-referential,
                                                                      max 2 levels)

┌──────────────────┐         ┌────────────────────────────────┐
│     Member       │         │      BorrowingTransaction      │
│──────────────────│         │────────────────────────────────│
│ PK Id            │ 1     * │ PK Id                          │
│    FirstName     ├────────►│ FK BookId                      │
│    LastName      │         │ FK MemberId                    │
│    Email (uniq)  │         │ FK BorrowedByUserId            │
│    Phone *       │         │ FK ReturnedByUserId *          │
│    MembershipDate│         │    BorrowDate                  │
│    IsActive      │         │    DueDate                     │
│ [soft-delete]    │         │    ReturnDate *                │
└──────────────────┘         │    Status (Active/Returned)    │
                             │ [soft-delete]                  │
                             └────────────┬───────────────────┘
                                          │ *
                             ┌────────────┴──────────┐
                             │      SystemUser        │
                             │────────────────────────│
                             │ PK Id                  │
                             │    Username (unique)   │
                             │    PasswordHash        │
                             │    Role (Admin/Lib/Stf)│
                             │    FullName            │
                             │    Email               │
                             │    IsActive            │
                             │ [soft-delete]          │
                             └────────────────────────┘

┌──────────────────────────────┐
│         ActivityLog          │
│──────────────────────────────│
│ PK Id                        │
│    UserId  (not FK)          │
│    Action                    │
│    EntityType                │
│    EntityId *                │
│    Timestamp                 │
│    Details (JSON) *          │
│  (append-only, no BaseEntity)│
└──────────────────────────────┘
```

## Relationship Summary

| Relationship | Cardinality | Notes |
|---|---|---|
| Publisher → Book | 1:N | One publisher, many books |
| Book ↔ Author | M:N | Via `BookAuthor` join table |
| Book ↔ Category | M:N | Via `BookCategory` join table |
| Category → Category | Self 1:N | `ParentCategoryId`, max 2 levels |
| Member → BorrowingTransaction | 1:N | One member, many borrows |
| Book → BorrowingTransaction | 1:N | One book, many transactions |
| SystemUser → BorrowingTransaction | 1:N | As `BorrowedByUser` and `ReturnedByUser` |

## BaseEntity Fields (inherited by all entities except join tables and ActivityLog)

| Field | Type | Description |
|---|---|---|
| `Id` | int | Auto-increment PK |
| `IsDeleted` | bool | Soft-delete flag; global EF filter `WHERE IsDeleted = 0` |
| `CreatedAt` | DateTime | UTC creation time |
| `CreatedBy` | string | Username or user ID of creator |
| `UpdatedAt` | DateTime? | UTC last update time |
| `UpdatedBy` | string? | Username or user ID of last updater |
