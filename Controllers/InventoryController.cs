using EcommerceAPI.Models.DTO.InventoryDTO;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
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
    [Route(Constants.Routes.CONTROLLER)]
    [Authorize(Roles = Constants.Roles.ADMIN)]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly string _email = string.Empty;

        public InventoryController(IInventoryService inventoryService, IHttpContextAccessor context)
        {
            _inventoryService = inventoryService;

            if (context.HttpContext.User.Identity != null)
            {
                _email = context.HttpContext.User.Identity.Name ?? string.Empty;
            }
       
        }


        [SwaggerOperation(summary: Constants.Swagger.Inventory.GET_INVENTORIES_SUMMARY, description: Constants.Swagger.Inventory.GET_INVENTORIES_DESCRIPTION)]
        [HttpGet]
        async public Task<IActionResult> GetInventories()
        {
            var inventories = await _inventoryService.GetInventoriesAsync();

            return Ok(inventories);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Inventory.GET_ALL_INVENTORY_OF_PRODUCT_SUMMARY, description: Constants.Swagger.Inventory.GET_ALL_INVENTORY_OF_PRODUCT_DESCRIPTION)]
        [HttpGet("{productId}")]
        async public Task<IActionResult> GetInventoryOfProduct(int productId)
        {
            var inventories = await _inventoryService.GetInventoryOfProductAsync(productId);
            return Ok(inventories);
        }


        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[SwaggerOperation(summary: Constants.Swagger.Inventory.GET_INVENTORY_SUMMARY, description: Constants.Swagger.Inventory.GET_INVENTORY_DESCRIPTION)]
        //[HttpGet("{productId}/{inventoryId}")]
        //async public Task<IActionResult> GetInventory(int productId, int inventoryId)
        //{
        //    var inventory = await _inventoryService.GetInventoryAsync(inventoryId);
        //    return Ok(inventory);
        //}


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Inventory.CREATE_INVENTORY_SUMMARY, description: Constants.Swagger.Inventory.CREATE_INVENTORY_DESCRIPTION)]
        [HttpPost]
        async public Task<IActionResult> CreateInventory(InventoryCreateDTO inventoryCreate)
        {
            var inventory = await _inventoryService.CreateInventoryAsync(inventoryCreate, _email);
            return Ok(inventory);
        }


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: Constants.Swagger.Inventory.UPDATE_INVENTORY_SUMMARY, description: Constants.Swagger.Inventory.UPDATE_INVENTORY_DESCRIPTION)]
        [HttpPut("{inventoryId}")]
        async public Task<IActionResult> UpdateInventory(int inventoryId, InventoryUpdateDTO inventoryUpdate)
        {
            var inventory = await _inventoryService.UpdateInventoryAsync(inventoryId, inventoryUpdate, _email);
            return Ok(inventory);
        }


        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(summary: Constants.Swagger.Inventory.DELETE_INVENTORY_SUMMARY, description: Constants.Swagger.Inventory.DELETE_INVENTORY_DESCRIPTION)]
        [HttpDelete("{inventoryId}")]
        async public Task<IActionResult> DeleteInventory(int inventoryId)
        {
            await _inventoryService.DeleteInventoryAsync(inventoryId);
            return NoContent();
        }
    }
}

