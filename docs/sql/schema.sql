IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [ActivityLogs] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [EntityType] nvarchar(100) NOT NULL,
        [EntityId] nvarchar(50) NULL,
        [Timestamp] datetime2 NOT NULL,
        [Details] nvarchar(max) NULL,
        CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [Authors] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Bio] nvarchar(max) NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Authors] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [Categories] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [ParentCategoryId] int NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Categories_Categories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [Categories] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [Members] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [Phone] nvarchar(20) NULL,
        [MembershipDate] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Members] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [Publishers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(300) NOT NULL,
        [Address] nvarchar(max) NULL,
        [Website] nvarchar(500) NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Publishers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [SystemUsers] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(100) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Role] nvarchar(max) NOT NULL,
        [FullName] nvarchar(200) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_SystemUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [Books] (
        [Id] int NOT NULL IDENTITY,
        [Title] nvarchar(500) NOT NULL,
        [ISBN] nvarchar(20) NOT NULL,
        [Edition] nvarchar(50) NULL,
        [Language] nvarchar(50) NULL,
        [PublicationYear] int NULL,
        [Summary] nvarchar(max) NULL,
        [CoverImagePath] nvarchar(500) NULL,
        [Status] nvarchar(max) NOT NULL,
        [PublisherId] int NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_Books] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Books_Publishers_PublisherId] FOREIGN KEY ([PublisherId]) REFERENCES [Publishers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [BookAuthors] (
        [BookId] int NOT NULL,
        [AuthorId] int NOT NULL,
        CONSTRAINT [PK_BookAuthors] PRIMARY KEY ([BookId], [AuthorId]),
        CONSTRAINT [FK_BookAuthors_Authors_AuthorId] FOREIGN KEY ([AuthorId]) REFERENCES [Authors] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookAuthors_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [BookCategories] (
        [BookId] int NOT NULL,
        [CategoryId] int NOT NULL,
        CONSTRAINT [PK_BookCategories] PRIMARY KEY ([BookId], [CategoryId]),
        CONSTRAINT [FK_BookCategories_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_BookCategories_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE TABLE [BorrowingTransactions] (
        [Id] int NOT NULL IDENTITY,
        [BookId] int NOT NULL,
        [MemberId] int NOT NULL,
        [BorrowedByUserId] int NOT NULL,
        [ReturnedByUserId] int NULL,
        [BorrowDate] datetime2 NOT NULL,
        [DueDate] datetime2 NOT NULL,
        [ReturnDate] datetime2 NULL,
        [Status] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [UpdatedBy] nvarchar(max) NULL,
        CONSTRAINT [PK_BorrowingTransactions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_BorrowingTransactions_Books_BookId] FOREIGN KEY ([BookId]) REFERENCES [Books] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BorrowingTransactions_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BorrowingTransactions_SystemUsers_BorrowedByUserId] FOREIGN KEY ([BorrowedByUserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_BorrowingTransactions_SystemUsers_ReturnedByUserId] FOREIGN KEY ([ReturnedByUserId]) REFERENCES [SystemUsers] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookAuthors_AuthorId] ON [BookAuthors] ([AuthorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BookCategories_CategoryId] ON [BookCategories] ([CategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Books_ISBN] ON [Books] ([ISBN]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Books_PublisherId] ON [Books] ([PublisherId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BorrowingTransactions_BookId] ON [BorrowingTransactions] ([BookId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BorrowingTransactions_BorrowedByUserId] ON [BorrowingTransactions] ([BorrowedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BorrowingTransactions_MemberId] ON [BorrowingTransactions] ([MemberId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_BorrowingTransactions_ReturnedByUserId] ON [BorrowingTransactions] ([ReturnedByUserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Categories_ParentCategoryId] ON [Categories] ([ParentCategoryId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Members_Email] ON [Members] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_SystemUsers_Username] ON [SystemUsers] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260626141816_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260626141816_InitialCreate', N'9.0.6');
END;

COMMIT;
GO

