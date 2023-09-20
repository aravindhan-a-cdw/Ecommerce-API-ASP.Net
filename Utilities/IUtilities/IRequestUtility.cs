using System;
using EcommerceAPI.Models.DTO.Product;

namespace EcommerceAPI.Utilities.IUtilities
{
	public interface IRequestUtility
	{
        public Task<List<BannedProductsDTO>> GetBannedProducts();

    }
}

