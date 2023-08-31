using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
	public class InventoryRepository: Repository<Inventory>
	{
        public InventoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Inventory?> GetAsync(System.Linq.Expressions.Expression<Func<Inventory, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<Inventory> query = dbSet;
            query = query.Include(record => record.Product).OrderBy(record => record.Price);

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

        public override async Task<List<Inventory>> GetAllAsync(System.Linq.Expressions.Expression<Func<Inventory, bool>>? Filter = null)
        {
            IQueryable<Inventory> query = dbSet;
            query = query.Include(record => record.Product);
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.ToListAsync();
        }
    }
}

