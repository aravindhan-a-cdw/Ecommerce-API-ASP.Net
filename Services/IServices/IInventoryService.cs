using EcommerceAPI.Models.DTO.InventoryDTO;

namespace EcommerceAPI.Services.IServices
{
    public interface IInventoryService
	{
        Task<List<InventoryPublicDTO>> GetInventoriesAsync();
        Task<InventoryPublicDTO> GetInventoryOfProductAsync(int productId);
        Task<InventoryPublicDTO> GetInventoryAsync(int inventoryId);
        Task<InventoryPublicDTO> CreateInventoryAsync(InventoryCreateDTO inventoryCreate, string adminEmail);
        Task<InventoryPublicDTO> UpdateInventoryAsync(int inventoryId, InventoryUpdateDTO inventoryUpdate, string adminEmail);
        Task<bool> DeleteInventoryAsync(int inventoryId);
    }
}

