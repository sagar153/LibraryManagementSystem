namespace LibraryManagementSystem.Data;

using Microsoft.EntityFrameworkCore;
using Models;

/// <summary>
/// Application database context - Database-First approach.
/// The database schema is managed externally via SQL scripts in Data/Scripts/.
/// Models are mapped to existing tables using Data Annotations ([Table], [Key], [ForeignKey]).
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User and Authentication
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    // Library Management
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<BookLoan> BookLoans { get; set; } = null!;
    public DbSet<BookReservation> BookReservations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Database-First: All schema configuration is defined in the database.
        // Table/column mappings are handled via Data Annotations on the model classes.
        // No Fluent API configuration is needed here.
    }
}
