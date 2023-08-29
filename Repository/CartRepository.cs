using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
	public class CartRepository: Repository<Cart>
    {
		public CartRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}

        public override async Task<Cart?> GetAsync(System.Linq.Expressions.Expression<Func<Cart, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<Cart> query = dbSet;
            query = query.Include(record => record.Product);
            if (NoTracking)
            {
                query = query.AsNoTracking();
            }

            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.FirstOrDefaultAsync();
        }

        public override async Task<List<Cart>> GetAllAsync(System.Linq.Expressions.Expression<Func<Cart, bool>>? Filter = null)
        {
            IQueryable<Cart> query = dbSet;
            query = query.Include(record => record.Product);
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.ToListAsync();
        }
    }
}

