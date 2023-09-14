using System;
namespace EcommerceAPI.Models.DTO.OrderDTO
{
	public class OrderPublicDTO: OrderBaseDTO
	{
		public int Id { get; set; }

		//public required new ICollection<OrderItemPublicDTO> OrderItems { get; set; }
	}
}

