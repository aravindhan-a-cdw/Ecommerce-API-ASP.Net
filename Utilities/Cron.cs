using System;
using System.Collections.Generic;
using EcommerceAPI.Data;
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace EcommerceAPI.Utilities
{

    public class CronJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CronJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var service = _serviceProvider.GetService<CartCleanJob>();
            if (service == null)
            {
                throw new Exception("CartCleanJob Service is not available");
            }
            return service;
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }

    public class CartCleanJob: IJob
    {
        private ApplicationDbContext _dbContext;
        public CartCleanJob(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Job executed at " + DateTime.Now);
            IQueryable<User> userQuery = _dbContext.Set<User>();
            IQueryable<Cart> cartQuery = _dbContext.Set<Cart>();
            userQuery = userQuery.Where(record => record.LastActive < (DateTime.UtcNow - TimeSpan.FromMinutes(2)));
            var inactiveUsers = await userQuery.ToListAsync();
            foreach (var user in inactiveUsers)
            {
                var toBeDeleted = await cartQuery.Where(record => record.UserId == user.Id).ToListAsync();
                if (toBeDeleted.Any())
                {
                    Console.WriteLine($"Deleting {toBeDeleted.Count} cart items for user {user.Email}");
                    // Delete the records
                    _dbContext.CartItems.RemoveRange(toBeDeleted);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}

