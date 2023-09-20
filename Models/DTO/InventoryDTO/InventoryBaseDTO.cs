using System;
namespace EcommerceAPI.Models.DTO.InventoryDTO
{
	public class InventoryBaseDTO
	{
        public int ProductId { get; set; }

        public required int QuantityAvailable { get; set; }

        public required int QuantitySold { get; set; } = 0;

        public required int Price { get; set; }
    }
}

