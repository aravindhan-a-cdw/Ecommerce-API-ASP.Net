using EcommerceAPI.Models.DTO.CategoryDTO;
using EcommerceAPI.Utilities;

namespace EcommerceAPI.Models.ProductDTO
{
	public class ProductPublicDTO: ProductBaseDTO
	{
		public required int Id { get; set; }
		public virtual CategoryBaseDTO Category { get; set; }
		public bool HasDisclaimer { get; set; } = false;
		public string _disclaimerMessage = string.Empty;
		public string DisclaimerMessage { get { return HasDisclaimer ? Constants.Messages.DISCLAIMER : string.Empty; } set { _disclaimerMessage = value; } }
	}
}

