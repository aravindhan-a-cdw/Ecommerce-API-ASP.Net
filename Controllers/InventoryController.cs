using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTO.InventoryDTO;
using EcommerceAPI.Repository;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EcommerceAPI.Controllers
{
    /*
     * @author Aravindhan A
     * @description This is the controller class for Inventory related Routes.
     */

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        [SwaggerOperation(summary: "Get all inventories of all products", description: "This endpoint allows admin to get all inventories of all products")]
        [HttpGet]
        async public Task<IActionResult> GetInventories()
        {
            var inventories = await _inventoryService.GetInventoriesAsync();

            return Ok(inventories);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Get All Inventory of a Product", description: "This endpoint allows admin to get all the inventory of products")]
        [HttpGet("{productId}")]
        async public Task<IActionResult> GetAllInventoryOfProduct(int productId)
        {
            var inventories = await _inventoryService.GetAllInventoryOfProductAsync(productId);
            return Ok(inventories);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Get a single Inventory with Id", description: "This endpoint allows admin to view Inventory of a Product")]
        [HttpGet("{productId}/{inventoryId}")]
        async public Task<IActionResult> GetInventory(int productId, int inventoryId)
        {
            var inventory = await _inventoryService.GetInventoryAsync(inventoryId);
            return Ok(inventory);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Create New Inventory for Product", description: "This endpoint allows admin to create New Inventory")]
        [HttpPost]
        async public Task<IActionResult> CreateInventory(InventoryCreateDTO inventoryCreate)
        {
            var userEmail = HttpContext.User.Identity.Name ?? "";

            var inventory = await _inventoryService.CreateInventoryAsync(inventoryCreate, userEmail);
            return Ok(inventory);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Update the inventory of a Product", description: "This endpoint allows Admin to update the inventory of a product")]
        [HttpPut("{inventoryId}")]
        async public Task<IActionResult> UpdateInventory(int inventoryId, InventoryUpdateDTO inventoryUpdate)
        {
            var userEmail = HttpContext.User.Identity.Name ?? "";

            var inventory = _inventoryService.UpdateInventoryAsync(inventoryId, inventoryUpdate, userEmail);
            return Ok(inventory);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: "Delete a Inventory for a Product", description: "This endpoint allows Admin to delete Inventory of a Product")]
        [HttpDelete("{inventoryId}")]
        async public Task<IActionResult> DeleteInventory(int inventoryId)
        {
            await _inventoryService.DeleteInventoryAsync(inventoryId);
            return NoContent();
        }
    }
}

