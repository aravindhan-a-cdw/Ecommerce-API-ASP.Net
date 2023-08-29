using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTO.InventoryDTO;
using EcommerceAPI.Repository;
using EcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly InventoryRepository _inventoryRepository;
        private readonly Repository<Product> _productRepository;
        private readonly IUserRepository _userRepository;

        public InventoryController(IMapper mapper, InventoryRepository inventoryRepository, Repository<Product> productRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        async public Task<IActionResult> GetInventories()
        {
            var inventories = await _inventoryRepository.GetAllAsync();

            return Ok(_mapper.Map<List<InventoryPublicDTO>>(inventories));
        }

        [HttpGet("{productId}")]
        async public Task<IActionResult> GetAllInventoryOfProduct(int productId)
        {
            var inventory = await _inventoryRepository.GetAllAsync(record => record.ProductId == productId);
            if (inventory == null)
            {
                return BadRequest("Record doesn't exist!");
            }
            return Ok(_mapper.Map<List<InventoryPublicDTO>>(inventory));
        }

        [HttpGet("{productId}/{inventoryId}")]
        async public Task<IActionResult> GetAInventory(int productId, int inventoryId)
        {
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId);
            if(inventory == null)
            {
                return NotFound("Record doesn't exist!");
            }
            return Ok(_mapper.Map<InventoryPublicDTO>(inventory));
        }

        [HttpPost]
        async public Task<IActionResult> CreateInventory(InventoryCreateDTO inventoryCreate)
        {
            var product = await _productRepository.GetAsync(record => record.Id == inventoryCreate.ProductId, NoTracking: true);
            if(product == null)
            {
                return BadRequest("No product found for your given input");
            }
            var user = HttpContext.User;
            var inventoryDb = _mapper.Map<Inventory>(inventoryCreate);
            var userDb = await _userRepository.GetAsync(record => record.Email == user.Identity.Name);
            if(userDb == null)
            {
                return BadRequest("User not found in database");
            }
            inventoryDb.CreatedBy = userDb;
            var inventory = await _inventoryRepository.CreateAsync(inventoryDb);
            return Ok(_mapper.Map<InventoryPublicDTO>(inventory));
        }

        [HttpPut("{inventoryId}")]
        async public Task<IActionResult> UpdateInventory(int inventoryId, InventoryUpdateDTO inventoryUpdate)
        {
            if(inventoryId != inventoryUpdate.Id)
            {
                return BadRequest("Id doesn't match in path and body");
            }
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId, NoTracking: true);
            if (inventory == null)
            {
                return NotFound("Record doesn't exist!");
            }
            var product = await _productRepository.GetAsync(record => record.Id == inventoryUpdate.ProductId, NoTracking: true);
            if (product == null)
            {
                return BadRequest("No product found for your given input");
            }
            var user = HttpContext.User;
            var inventoryDb = _mapper.Map<Inventory>(inventoryUpdate);
            var userDb = await _userRepository.GetAsync(record => record.Email == user.Identity.Name, NoTracking: true);
            if (userDb == null)
            {
                return BadRequest("User not found in database");
            }
            inventoryDb.CreatedBy = userDb;
            inventory = await _inventoryRepository.UpdateAsync(inventoryDb);
            return Ok(_mapper.Map<InventoryPublicDTO>(inventory));
        }

        [HttpDelete("{inventoryId}")]
        async public Task<IActionResult> DeleteInventory(int inventoryId)
        {
            var inventory = await _inventoryRepository.GetAsync(record => record.Id == inventoryId);
            if(inventory == null)
            {
                return NotFound("Record doesn't exist!");
            }
            await _inventoryRepository.RemoveAsync(inventory);
            return NoContent();
        }
    }
}

