using System;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection.Metadata;
using Microsoft.Extensions.Hosting;

namespace EcommerceAPI.Data
{
	public class ApplicationDbContext: IdentityDbContext<User>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions): base(dbContextOptions)
		{
		}

        //public DbSet<User> Users { get; set; }
        //public DbSet<Role> Roles { get; set; }

        public DbSet<User> ApplicationUsers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Inventory> Inventories { get; set; }

        public DbSet<OrderItems> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Add Created date on field creation
            modelBuilder.Entity<Order>()
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("now()");

            modelBuilder.Entity<Cart>()
                .HasKey(bc => new { bc.ProductId, bc.UserId });

            modelBuilder.Entity<OrderItems>().HasKey(record => new { record.OrderId, record.ProductId });

            modelBuilder.Entity<Cart>().HasOne(c => c.User).WithMany(c => c.CartItems).HasForeignKey(m => m.UserId);

            modelBuilder.Entity<Order>().HasOne(c => c.User).WithMany(c => c.Orders).HasForeignKey(m => m.UserId);

            //modelBuilder.Entity<Cart>()
            //    .HasOne(bc => bc.User)
            //    .WithMany(c => c.CartItems)
            //    .HasForeignKey(bc => bc.ProductId);

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

            //modelBuilder.Entity<Role>().HasData(
            //    new Role() { Id = 1, Name = "Admin", Description = "This Role allows the user to add products and inventory to the application." },
            //    new Role() { Id = 2, Name = "Customer", Description = "This Role allows the user to view and add product to cart and complete order." }
            //);

            //modelBuilder.Entity<User>().HasData(
            //    new User() { Id = 1, FirstName = "Demo", LastName = "Admin", Email = "admin@cdw.com", Password = "demo" },
            //    new User() { Id = 2, FirstName = "Demo", LastName = "Customer", Email = "customer@cdw.com", Password = "demo" }
            //);

            //modelBuilder.Entity<User>()
            //    .HasMany(p => p.Roles).WithMany(p => p.Users)
            //    .UsingEntity(
            //        j => j
            //        .ToTable("RoleUser")
            //        .HasData(new[] 
            //            {
            //                new { RolesId = 1, UsersId = 1},
            //                new {RolesId = 2, UsersId = 2}
            //            }
            //        )
            //    );

        }
    }
}

