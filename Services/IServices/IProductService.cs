using System;
using EcommerceAPI.Models.ProductDTO;

namespace EcommerceAPI.Services.IServices
{
	public interface IProductService
	{
        Task<List<ProductPublicDTO>> GetAllProductsAsync();
        Task<ProductPublicDTO> GetProductAsync(int productId);
        Task<ProductPublicDTO> CreateProductAsync(ProductCreateDTO productDto);
        Task<ProductPublicDTO> UpdateProductAsync(int productId, ProductUpdateDTO productUpdate);
        Task<bool> DeleteProductAsync(int productId);
    }
}

