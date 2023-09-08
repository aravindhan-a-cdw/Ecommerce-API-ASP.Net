
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceAPI.Models.DTO.CategoryDTO;
using Swashbuckle.AspNetCore.Annotations;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Controllers
{

    /*
     * @author Aravindhan A
     * @description This is the controller class for Category related Routes. CRUD operations can only be done by Admin users.
     */

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Controller for the routes related to Category creation, updation and deletion
        /// </summary>

        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        /// <summary>
        /// Route to Get all Categories and its related Products.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(summary:"Get all Categories and its related Products", description: "This endpoint provides all the categories and a list of all its related products")]
        async public Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }


        /// <summary>
        /// Route to get a Category and its related products
        /// </summary>
        [HttpGet("{categoryId}", Name = "GetCategory")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Get a Category and list of its related Products", description: "This endpoint gets a category and its list of related products")]
        async public Task<IActionResult> GetCategory([FromRoute] int categoryId)
        {
            var category = await _categoryService.GetCategoryAsync(categoryId);
            return Ok(category);
        }

        
        /// <summary>
        /// Route to Create a new Category
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Create a new Category", description: "This endpoint allows the admin to create a new category")]
        async public Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryCreate)
        {
            var category = await _categoryService.CreateCategoryAsync(categoryCreate);
            return Ok(category);
        }


        /// <summary>
        /// Route to Update a Category
        /// </summary>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Update a category by providing its id", description: "This endpoint allows the admin to update category")]
        [HttpPut("{categoryId}")]
        async public Task<IActionResult> UpdateCategory([FromRoute] int categoryId, [FromBody] CategoryUpdateDTO updateDTO)
        {
            var updatedCategory = await _categoryService.UpdateCategoryAsync(categoryId, updateDTO);
            return Ok(updatedCategory);
        }


        /// <summary>
        /// Route to Delete a Category
        /// </summary>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: "Delete a category by providing its id", description: "This endpoint allows the admin to delete a category")]
        [HttpDelete("{categoryId}")]
        async public Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
        {
            await _categoryService.DeleteCategoryAsync(categoryId);
            return NoContent();
        }

    }
}

