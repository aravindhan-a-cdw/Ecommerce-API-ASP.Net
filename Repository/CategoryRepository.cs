using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
	public class CategoryRepository: Repository<Category>
	{
		public CategoryRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}

        public override async Task<Category?> GetAsync(System.Linq.Expressions.Expression<Func<Category, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<Category> query = dbSet;

            if (NoTracking)
            {
                query = query.AsNoTracking();
            }

            if (Filter != null)
            {
                query = query.Include(record => record.Products).Where(Filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public override async Task<List<Category>> GetAllAsync(System.Linq.Expressions.Expression<Func<Category, bool>>? Filter = null)
        {
            IQueryable<Category> query = dbSet;
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.Include(record => record.Products).ToListAsync();
        }
    }
}

