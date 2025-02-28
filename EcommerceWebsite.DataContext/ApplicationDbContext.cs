using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EcommerceWebsite.EntityModels;
using Microsoft.AspNetCore.Identity;

namespace EcommerceWebsite.DataContext
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Category 1", Description = "no description" },
                new Category { Id = 2, Name = "Category 2", Description = "no description" },
                new Category { Id = 3, Name = "Category 3", Description = "no description" }
                );
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "Product 1", Description = "no description", Price = 300, CategoryId = 1, ImageURL = null },
                new Product { Id = 2, Title = "Product 2", Description = "no description", Price = 350, CategoryId = 1, ImageURL = null },
                new Product { Id = 3, Title = "Product 3", Description = "no description", Price = 400, CategoryId = 2, ImageURL = null },
                new Product { Id = 4, Title = "Product 4", Description = "no description", Price = 400, CategoryId = 2, ImageURL = null },
                new Product { Id = 5, Title = "Product 5", Description = "no description", Price = 400, CategoryId = 2, ImageURL = null },
                new Product { Id = 6, Title = "Product 6", Description = "no description", Price = 400, CategoryId = 3, ImageURL = null },
                new Product { Id = 7, Title = "Product 7", Description = "no description", Price = 400, CategoryId = 1, ImageURL = null },
                new Product { Id = 8, Title = "Product 8", Description = "no description", Price = 400, CategoryId = 2, ImageURL = null },
                new Product { Id = 9, Title = "Product 9", Description = "no description", Price = 400, CategoryId = 1, ImageURL = null }
                );
        }       
    }
}
