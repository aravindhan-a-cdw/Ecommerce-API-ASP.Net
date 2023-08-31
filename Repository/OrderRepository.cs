using System;
using EcommerceAPI.Data;
using EcommerceAPI.Models;

namespace EcommerceAPI.Repository
{
	public class OrderRepository: Repository<Order>
	{
		public OrderRepository(ApplicationDbContext applicationDbContext): base(applicationDbContext)
		{
		}
	}
}

