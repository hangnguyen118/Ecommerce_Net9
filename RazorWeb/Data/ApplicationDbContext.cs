using Microsoft.EntityFrameworkCore;
using RazorWeb.Models;

namespace RazorWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Book 1", Description = "no description" },
                new Category { Id = 2, Name = "Book 2", Description = "no description" },
                new Category { Id = 3, Name = "Book 3", Description = "no description" }
                );
        }
    }
}
