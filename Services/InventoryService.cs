using System;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTO.InventoryDTO;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Services
{
	public class InventoryService: IInventoryService
	{
        private readonly IRepository<Inventory> _inventoryRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public InventoryService(
            IRepository<Inventory> inventoryRepository,
            IRepository<Product> productRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<InventoryPublicDTO>> GetInventoriesAsync()
        {
            var inventories = await _inventoryRepository.GetAllAsync(Include: new() { "Product" });

            return _mapper.Map<List<InventoryPublicDTO>>(inventories);
        }

        public async Task<List<InventoryPublicDTO>> GetAllInventoryOfProductAsync(int productId)
        {
            var inventory = await _inventoryRepository.GetAllAsync(record => record.ProductId == productId, Include: new() { "Product" }) ?? throw new BadHttpRequestException("Record doesn't exist!");
            return _mapper.Map<List<InventoryPublicDTO>>(inventory);
        }

        public async Task<InventoryPublicDTO> GetInventoryAsync(int inventoryId)
        {
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId, Include: new() { "Product" }) ?? throw new BadHttpRequestException("Record doesn't exist!", StatusCodes.Status404NotFound);
            return _mapper.Map<InventoryPublicDTO>(inventory);
        }

        public async Task<InventoryPublicDTO> CreateInventoryAsync(InventoryCreateDTO inventoryCreate, string adminEmail)
        {
            var product = await _productRepository.GetAsync(record => record.Id == inventoryCreate.ProductId, NoTracking: true) ?? throw new BadHttpRequestException("No product found for your given input", StatusCodes.Status404NotFound);

            var inventoryDb = _mapper.Map<Inventory>(inventoryCreate);
            var userDb = await _userRepository.GetAsync(record => record.Email == adminEmail) ?? throw new BadHttpRequestException("User not found in database");
            inventoryDb.CreatedBy = userDb;
            var inventory = await _inventoryRepository.CreateAsync(inventoryDb);
            return _mapper.Map<InventoryPublicDTO>(inventory);
        }

        public async Task<InventoryPublicDTO> UpdateInventoryAsync(int inventoryId, InventoryUpdateDTO inventoryUpdate, string adminEmail)
        {
            if (inventoryId != inventoryUpdate.Id)
            {
                throw new BadHttpRequestException("Id doesn't match in path and body");
            }
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId, NoTracking: true) ?? throw new BadHttpRequestException("Record doesn't exist!", StatusCodes.Status404NotFound);
            var product = await _productRepository.GetAsync(record => record.Id == inventoryUpdate.ProductId, NoTracking: true) ?? throw new BadHttpRequestException("Record doesn't exist!", StatusCodes.Status404NotFound);
            var inventoryDb = _mapper.Map<Inventory>(inventoryUpdate);

            var userDb = await _userRepository.GetAsync(record => record.Email == adminEmail, NoTracking: true) ?? throw new BadHttpRequestException("User doesn't exist!", StatusCodes.Status404NotFound);
            inventoryDb.CreatedBy = userDb;
            inventory = await _inventoryRepository.UpdateAsync(inventoryDb);
            return _mapper.Map<InventoryPublicDTO>(inventory);
        }

        public async Task<bool> DeleteInventoryAsync(int inventoryId)
        {
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId);
            if (inventory == null)
            {
                throw new BadHttpRequestException("Record doesn't exist!", StatusCodes.Status404NotFound);
            }
            await _inventoryRepository.RemoveAsync(inventory);
            return true;
        }
    }
}

