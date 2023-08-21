using System;
using EcommerceAPI.Models;
using EcommerceAPI.Data;

namespace EcommerceAPI.Repository
{
	public class RoleRepository: Repository<Role>
	{
		public RoleRepository(ApplicationDbContext dbContext): base(dbContext)
		{
		}
	}
}

