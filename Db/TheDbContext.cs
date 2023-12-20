using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Db
{
    public class TheDbContext : DbContext
    {
        public TheDbContext(DbContextOptions<TheDbContext> options) : base(options) { }

        public DbSet<Password> Passwords { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<TimeUnit> TimeUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

            // Categories table
            modelBuilder.Entity<Category>()
                .ToTable("Categories") // Create Table
                .HasKey(c => c.Category_id); // Primary key

            // Passwords table
            modelBuilder.Entity<Password>()
                .ToTable("Passwords")
                .HasKey(p => p.Password_id); // Primary key

            // Relationship between Passwords and Categories
            modelBuilder.Entity<Password>()
                .HasOne(p => p.Category) // A Password has One Category
                .WithMany(c => c.Passwords) // One Category can have many Passwords
                .HasForeignKey(p => p.Category_id); // Foreign key in Passwords

            // Relationship between Passwords and Timeunits
            modelBuilder.Entity<Password>()
                .HasOne(p => p.TimeUnit) // A Password has One Timunit
                .WithMany(t => t.Passwords) // One Timeunit can have many Passwords
                .HasForeignKey(p => p.Time_unit_id); // Foreign key in Passwords

            // Time_units table
            modelBuilder.Entity<TimeUnit>()
                .ToTable("Time_units")
                .HasKey(t => t.Time_unit_id); // Primary Key
        }
    }
}