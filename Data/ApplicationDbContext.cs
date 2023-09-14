using System;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;

namespace EcommerceAPI.Data
{
    /*
     * @author Aravindhan A
     * @description This is the Dbcontext which have the table models and table setup codes.
     */
    public class ApplicationDbContext: IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions): base(dbContextOptions)
		{
		}

        public DbSet<User> ApplicationUsers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<User>().Property(b => b.LastActive).HasDefaultValue(DateTime.UtcNow);

            modelBuilder.Entity<Cart>()
                .HasKey(bc => new { bc.ProductId, bc.UserId });

            modelBuilder.Entity<OrderItem>().HasKey(record => new { record.OrderId, record.ProductId });

            modelBuilder.Entity<Cart>().HasOne(c => c.User).WithMany(c => c.CartItems).HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Order>().HasOne(c => c.User).WithMany(c => c.Orders).HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Category>().HasIndex(record => record.Name).IsUnique();

            modelBuilder.Entity<Category>()
                .HasMany(e => e.Products)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
            .IsRequired();

            modelBuilder.Entity<Product>()
            .HasOne(e => e.Category)
            .WithMany(e => e.Products)
            .HasForeignKey(e => e.CategoryId)
            .IsRequired();         

        }
    }
}

