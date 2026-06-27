using LibraryMS.Application.Services.Interfaces;
using LibraryMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Data;

public class LibraryDbContext : DbContext, ILibraryDbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Publisher> Publishers => Set<Publisher>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<SystemUser> SystemUsers => Set<SystemUser>();
    public DbSet<BorrowingTransaction> BorrowingTransactions => Set<BorrowingTransaction>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

        // Global soft-delete filter for all BaseEntity descendants
        modelBuilder.Entity<Book>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Author>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Publisher>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Member>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SystemUser>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<BorrowingTransaction>().HasQueryFilter(e => !e.IsDeleted);
    }
}
