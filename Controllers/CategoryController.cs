using EcommerceAPI.Models.DTO.CategoryDTO;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EcommerceAPI.Controllers
{

    /*
     * @author Aravindhan A
     * @description This is the controller class for Category related Routes. CRUD operations can only be done by Admin users.
     */

    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = Constants.Roles.ADMIN)]
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
        [SwaggerOperation(summary: Constants.Swagger.Category.GET_ALL_SUMMARY, description: Constants.Swagger.Category.GET_ALL_DESCRIPTION)]
        async public Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }


        /// <summary>
        /// Route to get a Category and its related products
        /// </summary>
        [HttpGet("{categoryId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Category.GET_CATEGORY_SUMMARY, description: Constants.Swagger.Category.GET_CATEGORY_DESCRIPTION)]
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
        [SwaggerOperation(summary: Constants.Swagger.Category.CREATE_CATEGORY_SUMMARY, description: Constants.Swagger.Category.CREATE_CATEGORY_DESCRIPTION)]
        async public Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryCreate)
        {
            var category = await _categoryService.CreateCategoryAsync(categoryCreate);
            return Ok(category);
        }


        /// <summary>
        /// Route to Update a Category
        /// </summary>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(summary: Constants.Swagger.Category.UPDATE_CATEGORY_SUMMARY, description: Constants.Swagger.Category.UPDATE_CATEGORY_DESCRIPTION)]
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
        [SwaggerOperation(summary: Constants.Swagger.Category.DELETE_CATEGORY_SUMMARY, description: Constants.Swagger.Category.DELETE_CATEGORY_DESCRIPTION)]
        [HttpDelete("{categoryId}")]
        async public Task<IActionResult> DeleteCategory([FromRoute] int categoryId)
        {
            await _categoryService.DeleteCategoryAsync(categoryId);
            return NoContent();
        }

    }
}

