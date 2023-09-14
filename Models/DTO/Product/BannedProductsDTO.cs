using System;
namespace EcommerceAPI.Models.DTO.Product
{
	public class BannedProductsDTO
	{
		public required int id { get; set; }
		public required string productId { get; set; }
        public required bool isBanned { get; set; }
        public required bool isTrialGoing { get; set; }
    }
}

