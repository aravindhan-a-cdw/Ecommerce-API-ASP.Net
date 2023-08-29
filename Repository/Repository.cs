﻿using System;
using EcommerceAPI.Data;
using EcommerceAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            var dbEntity = await dbSet.AddAsync(entity);
            await SaveAsync();
            return entity;

        }

        public virtual async Task<T?> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>> Filter, bool NoTracking = false)
        {
            IQueryable<T> query = dbSet;

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

        public virtual async Task<List<T>> GetAllAsync(System.Linq.Expressions.Expression<Func<T, bool>>? Filter = null)
        {
            IQueryable<T> query = dbSet;
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            return await query.ToListAsync();
        }

        public virtual async Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
        }

        async virtual public Task<T> UpdateAsync(T entity)
        {
            //entity = DateTime.UtcNow;
            dbSet.Update(entity);
            await SaveAsync();
            return entity;
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}

