
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceAPI.Models.DTO.CategoryDTO;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "admin")]
    public class CategoryController : ControllerBase
    {
        private readonly Repository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(Repository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        async public Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return Ok(_mapper.Map<List<CategoryPublicDTO>>(categories));
        }

        [HttpGet("{id}", Name = "GetCategory")]
        [AllowAnonymous]
        async public Task<IActionResult> GetCategory([FromRoute] int id)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == id);
            if (category == null)
            {
                return BadRequest("Record not found!");
            }
            return Ok(_mapper.Map<CategoryPublicDTO>(category));
        }

        
        [HttpPost]
        async public Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO categoryCreate)
        {
            var category = await _categoryRepository.CreateAsync(_mapper.Map<Category>(categoryCreate));
            return CreatedAtAction("GetCategory", new{ id = category.Id}, category);
        }

        
        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryUpdateDTO updateDTO)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == id, true);
            if(category == null)
            {
                return BadRequest("Record doesn't exist");
            }
            category.Name = updateDTO.Name;
            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return Ok(updatedCategory);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        async public Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == id);
            if (category == null)
            {
                return BadRequest("Record doesn't exist");
            }
            await _categoryRepository.RemoveAsync(category);
            return NoContent();
        }

    }
}

