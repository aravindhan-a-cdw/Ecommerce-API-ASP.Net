using System;
using EcommerceAPI.Models.CartDTO;
using EcommerceAPI.Models.DTO.OrderDTO;

namespace EcommerceAPI.Services.IServices
{
    public interface ICartService
    {
        Task<List<CartPublicDTO>> GetCartItemsAsync(string userEmail);
        Task<CartPublicDTO> AddProductToCartAsync(string userEmail, CartItemAddDTO itemAddDTO);
        Task<CartPublicDTO> UpdateProductInCartAsync(string userEmail, CartItemAddDTO itemAddDTO);
        Task<bool> DeleteProductInCartAsync(string userEmail, int productId);
        Task<OrderPublicDTO> ProceedToCheckoutAsync(string userEmail);
    }
}

