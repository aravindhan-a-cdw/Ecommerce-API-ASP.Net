using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repository
{
	public class OrderItemsRepository: Repository<OrderItem>
	{
		public OrderItemsRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}
	}
}

