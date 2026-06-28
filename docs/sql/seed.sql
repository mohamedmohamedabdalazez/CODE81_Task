-- ============================================================
-- Library Management System — Sample Seed Data
-- Run AFTER schema.sql / EF migrations have been applied.
-- Passwords are BCrypt hashes of: Admin@123, Librarian@123, Staff@123
-- ============================================================

-- System Users (3 per role)
INSERT INTO SystemUsers (Username, PasswordHash, Role, FullName, Email, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES
-- Administrators (password: Admin@123)
('admin1',      '$2a$11$DC5zJGAeZiNEU9safAF0XuGeoAVQYAoemW0/exW1sXgxW8kSOI7qG', 'Administrator', 'Alice Administrator', 'alice@library.com', 1, 0, GETUTCDATE(), 'system'),
('admin2',      '$2a$11$DC5zJGAeZiNEU9safAF0XuGeoAVQYAoemW0/exW1sXgxW8kSOI7qG', 'Administrator', 'Bob Admin',           'bob@library.com',   1, 0, GETUTCDATE(), 'system'),
('admin3',      '$2a$11$DC5zJGAeZiNEU9safAF0XuGeoAVQYAoemW0/exW1sXgxW8kSOI7qG', 'Administrator', 'Carol Admin',         'carol@library.com', 1, 0, GETUTCDATE(), 'system'),
-- Librarians (password: Librarian@123)
('librarian1',  '$2a$11$Fy7G2dQOyjemYaVtU0phS.nK2otKyJcAyUJpDn3SoMnFkeZxj6ODm', 'Librarian',     'David Librarian',     'david@library.com', 1, 0, GETUTCDATE(), 'system'),
('librarian2',  '$2a$11$Fy7G2dQOyjemYaVtU0phS.nK2otKyJcAyUJpDn3SoMnFkeZxj6ODm', 'Librarian',     'Eve Librarian',       'eve@library.com',   1, 0, GETUTCDATE(), 'system'),
('librarian3',  '$2a$11$Fy7G2dQOyjemYaVtU0phS.nK2otKyJcAyUJpDn3SoMnFkeZxj6ODm', 'Librarian',     'Frank Librarian',     'frank@library.com', 1, 0, GETUTCDATE(), 'system'),
-- Staff (password: Staff@123)
('staff1',      '$2a$11$LqJ/4PcRR8KAOAVBssKlMOTD1gW6khxX1BAmkBRMO3TTg3UKtKFe.', 'Staff',         'Grace Staff',         'grace@library.com', 1, 0, GETUTCDATE(), 'system'),
('staff2',      '$2a$11$LqJ/4PcRR8KAOAVBssKlMOTD1gW6khxX1BAmkBRMO3TTg3UKtKFe.', 'Staff',         'Henry Staff',         'henry@library.com', 1, 0, GETUTCDATE(), 'system'),
('staff3',      '$2a$11$LqJ/4PcRR8KAOAVBssKlMOTD1gW6khxX1BAmkBRMO3TTg3UKtKFe.', 'Staff',         'Iris Staff',          'iris@library.com',  1, 0, GETUTCDATE(), 'system');

-- Publishers
INSERT INTO Publishers (Name, Address, Website, IsDeleted, CreatedAt, CreatedBy)
VALUES
('Penguin Random House', 'New York, NY', 'https://penguinrandomhouse.com', 0, GETUTCDATE(), 'system'),
('HarperCollins',        'New York, NY', 'https://harpercollins.com',      0, GETUTCDATE(), 'system'),
('O''Reilly Media',      'Sebastopol, CA', 'https://oreilly.com',          0, GETUTCDATE(), 'system'),
('MIT Press',            'Cambridge, MA', 'https://mitpress.mit.edu',      0, GETUTCDATE(), 'system'),
('Bloomsbury',           'London, UK',   'https://bloomsbury.com',         0, GETUTCDATE(), 'system');

-- Authors
INSERT INTO Authors (FirstName, LastName, Bio, IsDeleted, CreatedAt, CreatedBy)
VALUES
('George',    'Orwell',       'English novelist known for 1984 and Animal Farm.',                  0, GETUTCDATE(), 'system'),
('J.K.',      'Rowling',      'British author of the Harry Potter series.',                        0, GETUTCDATE(), 'system'),
('Martin',    'Fowler',       'Software architect and author on enterprise patterns.',             0, GETUTCDATE(), 'system'),
('Robert',    'Martin',       'Author of Clean Code and Clean Architecture.',                      0, GETUTCDATE(), 'system'),
('Frank',     'Herbert',      'American science fiction author, creator of Dune.',                 0, GETUTCDATE(), 'system'),
('Tolkien',   'J.R.R.',       'English author of The Lord of the Rings.',                          0, GETUTCDATE(), 'system'),
('Isaac',     'Asimov',       'Prolific science fiction writer and biochemistry professor.',       0, GETUTCDATE(), 'system'),
('Andrew',    'Hunt',         'Co-author of The Pragmatic Programmer.',                            0, GETUTCDATE(), 'system'),
('David',     'Thomas',       'Co-author of The Pragmatic Programmer.',                            0, GETUTCDATE(), 'system'),
('Harper',    'Lee',          'American novelist, author of To Kill a Mockingbird.',              0, GETUTCDATE(), 'system');

-- Categories (top-level)
INSERT INTO Categories (Name, ParentCategoryId, IsDeleted, CreatedAt, CreatedBy)
VALUES
('Fiction',              NULL, 0, GETUTCDATE(), 'system'),
('Non-Fiction',          NULL, 0, GETUTCDATE(), 'system'),
('Technology',           NULL, 0, GETUTCDATE(), 'system'),
('Science',              NULL, 0, GETUTCDATE(), 'system');

-- Sub-categories (child level)
INSERT INTO Categories (Name, ParentCategoryId, IsDeleted, CreatedAt, CreatedBy)
VALUES
('Science Fiction',   1, 0, GETUTCDATE(), 'system'),
('Fantasy',           1, 0, GETUTCDATE(), 'system'),
('Classic Fiction',   1, 0, GETUTCDATE(), 'system'),
('Software Engineering', 3, 0, GETUTCDATE(), 'system'),
('Computer Science',     3, 0, GETUTCDATE(), 'system'),
('Physics',              4, 0, GETUTCDATE(), 'system');

-- Books (PublisherId references: 1=Penguin, 2=Harper, 3=O'Reilly, 4=MIT, 5=Bloomsbury)
INSERT INTO Books (Title, ISBN, Edition, Language, PublicationYear, Summary, Status, PublisherId, IsDeleted, CreatedAt, CreatedBy)
VALUES
('1984',                        '978-0451524935', '1st', 'English', 1949,  'A dystopian social science fiction novel.',          'Available', 1, 0, GETUTCDATE(), 'system'),
('Animal Farm',                 '978-0451526342', '1st', 'English', 1945,  'An allegorical novella reflecting Soviet events.',   'Available', 1, 0, GETUTCDATE(), 'system'),
("Harry Potter and the Philosopher's Stone", '978-0439708180', '1st', 'English', 1997, 'The first Harry Potter novel.',          'Available', 5, 0, GETUTCDATE(), 'system'),
('Dune',                        '978-0441013593', '1st', 'English', 1965,  'A landmark science fiction epic.',                  'Available', 1, 0, GETUTCDATE(), 'system'),
('The Lord of the Rings',       '978-0544003415', '50th Anniversary', 'English', 1954, 'Epic high fantasy novel trilogy.',       'Available', 2, 0, GETUTCDATE(), 'system'),
('Foundation',                  '978-0553293357', '1st', 'English', 1951,  'First novel of the Foundation series.',             'Available', 1, 0, GETUTCDATE(), 'system'),
('To Kill a Mockingbird',       '978-0061935466', '1st', 'English', 1960,  'A novel about racial injustice in the Deep South.', 'Available', 2, 0, GETUTCDATE(), 'system'),
('Clean Code',                  '978-0132350884', '1st', 'English', 2008,  'A handbook of agile software craftsmanship.',       'Available', 2, 0, GETUTCDATE(), 'system'),
('Refactoring',                 '978-0201485677', '2nd', 'English', 1999,  'Improving the design of existing code.',            'Available', 3, 0, GETUTCDATE(), 'system'),
('The Pragmatic Programmer',    '978-0135957059', '20th Anniversary', 'English', 1999, 'A practical guide to software development.', 'Available', 2, 0, GETUTCDATE(), 'system'),
('Clean Architecture',          '978-0134494166', '1st', 'English', 2017,  'A craftsman guide to software structure.',          'Available', 2, 0, GETUTCDATE(), 'system'),
('Design Patterns',             '978-0201633610', '1st', 'English', 1994,  'Elements of reusable object-oriented software.',    'Available', 3, 0, GETUTCDATE(), 'system');

-- BookAuthors
INSERT INTO BookAuthors (BookId, AuthorId) VALUES
(1,  1), -- 1984 -> Orwell
(2,  1), -- Animal Farm -> Orwell
(3,  2), -- HP -> Rowling
(4,  5), -- Dune -> Herbert
(5,  6), -- LOTR -> Tolkien
(6,  7), -- Foundation -> Asimov
(7,  10), -- TKAM -> Harper Lee
(8,  4), -- Clean Code -> R. Martin
(9,  3), -- Refactoring -> Fowler
(10, 8), -- Pragmatic -> Hunt
(10, 9), -- Pragmatic -> Thomas
(11, 4), -- Clean Architecture -> R. Martin
(12, 3); -- Design Patterns -> Fowler (simplified)

-- BookCategories  (sub-category IDs: 5=SciFi, 6=Fantasy, 7=Classic, 8=SoftEng, 9=CS)
INSERT INTO BookCategories (BookId, CategoryId) VALUES
(1,  7), -- 1984 -> Classic Fiction
(2,  7), -- Animal Farm -> Classic Fiction
(3,  6), -- HP -> Fantasy
(4,  5), -- Dune -> Science Fiction
(5,  6), -- LOTR -> Fantasy
(6,  5), -- Foundation -> Science Fiction
(7,  7), -- TKAM -> Classic Fiction
(8,  8), -- Clean Code -> Software Engineering
(9,  8), -- Refactoring -> Software Engineering
(10, 8), -- Pragmatic -> Software Engineering
(11, 8), -- Clean Architecture -> Software Engineering
(12, 9); -- Design Patterns -> Computer Science

-- Members
INSERT INTO Members (FirstName, LastName, Email, Phone, MembershipDate, IsActive, IsDeleted, CreatedAt, CreatedBy)
VALUES
('John',   'Doe',     'john.doe@email.com',     '555-0101', '2024-01-15', 1, 0, GETUTCDATE(), 'system'),
('Jane',   'Smith',   'jane.smith@email.com',   '555-0102', '2024-02-20', 1, 0, GETUTCDATE(), 'system'),
('Mike',   'Johnson', 'mike.j@email.com',       '555-0103', '2024-03-10', 1, 0, GETUTCDATE(), 'system'),
('Sara',   'Williams','sara.w@email.com',        '555-0104', '2024-04-05', 1, 0, GETUTCDATE(), 'system'),
('Tom',    'Brown',   'tom.b@email.com',         '555-0105', '2024-05-18', 1, 0, GETUTCDATE(), 'system'),
('Emily',  'Davis',   'emily.d@email.com',       '555-0106', '2024-06-01', 1, 0, GETUTCDATE(), 'system');

-- Borrowing Transactions (Book 1 = borrowed, others available)
INSERT INTO BorrowingTransactions (BookId, MemberId, BorrowedByUserId, BorrowDate, DueDate, Status, IsDeleted, CreatedAt, CreatedBy)
VALUES
(1, 1, 1, DATEADD(day, -10, GETUTCDATE()), DATEADD(day, 4, GETUTCDATE()), 'Active',    0, GETUTCDATE(), 'system');

-- Update Book 1 status to Borrowed
UPDATE Books SET Status = 'Borrowed' WHERE Id = 1;

-- Activity Logs
INSERT INTO ActivityLogs (UserId, Action, EntityType, EntityId, Timestamp, Details)
VALUES
(1, 'SeedData', 'System', NULL, GETUTCDATE(), '{"note":"Initial seed data loaded"}');
