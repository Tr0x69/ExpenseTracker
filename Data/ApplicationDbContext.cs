using ExpenseTracker.Models.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<Applicationuser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Expense> Expenses { get; set; } = null!;

        public DbSet<Category> Category { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>().HasData(
                new Category {  CategoryId = 1, Name="Food"},
                new Category {  CategoryId = 2, Name="Transport"},
                new Category {  CategoryId = 3, Name="Entertainment"}
                );
        }
    }
}