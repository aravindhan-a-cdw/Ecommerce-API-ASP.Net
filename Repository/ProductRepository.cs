using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
	public class ProductRepository: Repository<Product>
	{
		public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
		}

        public override async Task<Product?> GetAsync(System.Linq.Expressions.Expression<Func<Product, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<Product> query = dbSet;

            if (NoTracking)
            {
                query = query.AsNoTracking();
            }

            if (Filter != null)
            {
                query = query.Include(record => record.Category).Where(Filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public override async Task<List<Product>> GetAllAsync(System.Linq.Expressions.Expression<Func<Product, bool>>? Filter = null)
        {
            IQueryable<Product> query = dbSet;
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.Include(record => record.Category).ToListAsync();
        }
    }
}

