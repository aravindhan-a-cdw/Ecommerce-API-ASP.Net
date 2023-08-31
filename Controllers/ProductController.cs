
using Microsoft.AspNetCore.Mvc;
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Repository;
using EcommerceAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly Repository<Product> _productRepository;
        private readonly Repository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductController(Repository<Product> productRepository, Repository<Category> categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        [HttpGet]
        [ResponseCache(Duration=60)]
        async public Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(_mapper.Map<List<ProductPublicDTO>>(products));
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        async public Task<IActionResult> GetProduct([FromRoute] int id)
        {
            var product = await _productRepository.GetAsync(record => record.Id == id);
            if(product == null)
            {
                return NotFound("Record not found!");
            }
            return Ok(_mapper.Map<ProductPublicDTO>(product));
        }


        [HttpPost]
        [Authorize(Roles = "admin")]
        async public Task<IActionResult> CreateProduct([FromBody] ProductCreateDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var category = await _categoryRepository.GetAsync(record => record.Id == productDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Category Doesn't exist");
            }
            //product.Category = category;
            var productDb = await _productRepository.CreateAsync(product);
            return CreatedAtRoute("GetProduct", new { id = productDb.Id }, _mapper.Map<ProductPublicDTO>(productDb));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        async public Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductUpdateDTO productUpdate)
        {
            var product = await _productRepository.GetAsync(record => record.Id == id, true);
            if(product == null)
            {
                return NotFound("Record not found!");
            }

            product.Name = productUpdate.Name;
            product.Description = productUpdate.Description;
            product.CategoryId = productUpdate.CategoryId;
            product.Images = productUpdate.Images;
            Product updated = await _productRepository.UpdateAsync(product);
            return Ok(_mapper.Map<ProductPublicDTO>(updated));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        async public Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var product = await _productRepository.GetAsync(record => record.Id == id, true);
            if (product == null)
            {
                return NotFound("Record not found!");
            }
            await _productRepository.RemoveAsync(product);
            return NoContent();
        }

    }
}

