using EcommerceAPI.Data;

namespace EcommerceAPI.Utilities
{
    public class CustomerUtility
	{
        private readonly ApplicationDbContext _db;

		public CustomerUtility(ApplicationDbContext db)
		{
            _db = db;
		}

		public void UpdateLastAccess(string userEmail)
		{
			var user = _db.ApplicationUsers.FirstOrDefault(q => q.Email == userEmail);
			if(user == null)
			{
                return;
			}
			user.LastActive = DateTime.UtcNow;
			_db.ApplicationUsers.Update(user);
			_db.SaveChanges();
		}
	}
}

