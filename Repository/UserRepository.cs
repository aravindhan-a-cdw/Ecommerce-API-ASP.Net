using System;
using EcommerceAPI.Models;
using EcommerceAPI.Data;

namespace EcommerceAPI.Repository
{
	public class UserRepository : Repository<User>
	{
		public UserRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}
	}
}

