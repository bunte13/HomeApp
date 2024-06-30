using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HomeApp.Models;

namespace HomeApp.Data
{
    public class HomeAppContext : IdentityDbContext<HomeAppUser>
    {
        public HomeAppContext(DbContextOptions<HomeAppContext> options)
            : base(options)
        {
        }

        // DbSet properties for your application entities
        public DbSet<Item> Item { get; set; }
        public DbSet<CategoryItem> CategoryItem { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<UserItem> UserItem { get; set; }
    }
}
