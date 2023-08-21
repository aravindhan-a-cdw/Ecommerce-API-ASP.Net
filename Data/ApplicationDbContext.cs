using System;
using Microsoft.EntityFrameworkCore;
using EcommerceAPI.Models;
using Microsoft.Extensions.Hosting;

namespace EcommerceAPI.Data
{
	public class ApplicationDbContext: DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> dbContextOptions): base(dbContextOptions)
		{
		}

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Role>().HasData(
                new Role() { Id = 1, Name = "Admin", Description = "This Role allows the user to add products and inventory to the application." },
                new Role() { Id = 2, Name = "Customer", Description = "This Role allows the user to view and add product to cart and complete order." }
            );

            modelBuilder.Entity<User>().HasData(
                new User() { Id = 1, FirstName = "Demo", LastName = "Admin", Email = "admin@cdw.com", Password = "demo" },
                new User() { Id = 2, FirstName = "Demo", LastName = "Customer", Email = "customer@cdw.com", Password = "demo" }
            );

            modelBuilder.Entity<User>()
                .HasMany(p => p.Roles).WithMany(p => p.Users)
                .UsingEntity(
                    j => j
                    .ToTable("RoleUser")
                    .HasData(new[] 
                        {
                            new { RolesId = 1, UsersId = 1},
                            new {RolesId = 2, UsersId = 2}
                        }
                    )
                );

        }
    }
}

