using System;
using System.Linq.Expressions;
using EcommerceAPI.Models;
using EcommerceAPI.Models.ProductDTO;

namespace EcommerceAPI.Services.IServices
{
	public interface IProductService
	{
        Task<List<ProductPublicDTO>> GetAllProductsAsync(Expression<Func<Product, bool>>? filter);
        Task<ProductPublicDTO> GetProductAsync(int productId);
        Task<ProductPublicDTO> CreateProductAsync(ProductCreateDTO productDto);
        Task<ProductPublicDTO> UpdateProductAsync(int productId, ProductUpdateDTO productUpdate);
        Task<bool> DeleteProductAsync(int productId);
    }
}

