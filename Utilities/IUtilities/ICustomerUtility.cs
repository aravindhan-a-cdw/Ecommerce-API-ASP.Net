using System;
namespace EcommerceAPI.Utilities.IUtilities
{
	public interface ICustomerUtility
	{
        public void UpdateLastAccess(string userEmail);
    }
}

