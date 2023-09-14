using System;
using EcommerceAPI.Models.DTO.Product;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Utilities
{
	public class RequestsUtility
	{
		private readonly IMemoryCache _memoryCache;
        private const string BANNED_PRODUCTS_KEY = "BannedProducts";

        public RequestsUtility(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		public async Task<List<BannedProductsDTO>> GetBannedProducts()
		{
            var data = _memoryCache.Get<List<BannedProductsDTO>>(BANNED_PRODUCTS_KEY);
            if (data == null)
            {
                var client = new HttpClient() { BaseAddress = new Uri("https://65014f45736d26322f5b7b24.mockapi.io/cosmo/ecart") };
                data = await client.GetFromJsonAsync<List<BannedProductsDTO>>("") ?? new();
                _memoryCache.Set(BANNED_PRODUCTS_KEY, data, TimeSpan.FromSeconds(Constants.CacheExpiration.BANNED_PRODUCTS_EXPIRY));
            }
            return data;
        }
	}
}

