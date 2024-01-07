using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shoper.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Shoper.Data
{
	public class ApplicationDbContext : IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<Shoper.Models.Product> Product { get; set; }
		public DbSet<Shoper.Models.ShoppingList> ShoppingList { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
	            .HasMany(u => u.ShoppingLists)
	            .WithOne(sl => sl.Owner)
	            .HasForeignKey(sl => sl.OwnerId)
	            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShoppingList>()
				.HasMany(s => s.Products)
				.WithOne(p => p.ShoppingList)
				.OnDelete(DeleteBehavior.Cascade);
        }
    }
}