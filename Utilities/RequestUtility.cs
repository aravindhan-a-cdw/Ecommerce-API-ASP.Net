using EcommerceAPI.Models.DTO.Product;
using EcommerceAPI.Utilities.IUtilities;
using Microsoft.Extensions.Caching.Memory;

namespace EcommerceAPI.Utilities
{
    public class RequestUtility: IRequestUtility
	{
		private readonly IMemoryCache _memoryCache;
        private const string BANNED_PRODUCTS_KEY = "BannedProducts";

        public RequestUtility(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		public async Task<List<BannedProductsDTO>> GetBannedProducts()
		{
            var data = _memoryCache.Get<List<BannedProductsDTO>>(BANNED_PRODUCTS_KEY);
            if (data == null)
            {
                var client = new HttpClient();
                data = await client.GetFromJsonAsync<List<BannedProductsDTO>>(Constants.Utility.BANNED_PRODUCTS_URL) ?? new();
                _memoryCache.Set(BANNED_PRODUCTS_KEY, data, TimeSpan.FromSeconds(Constants.CacheExpiration.BANNED_PRODUCTS_EXPIRY));
            }
            return data;
        }
	}
}

