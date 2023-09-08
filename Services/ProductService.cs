using System;
using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.ProductDTO;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Services
{
	public class ProductService: IProductService
	{
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public ProductService(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<ProductPublicDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync(Include: new() { "Category"});
            return _mapper.Map<List<ProductPublicDTO>>(products);
        }

        public async Task<ProductPublicDTO> GetProductAsync(int productId)
        {
            var product = await _productRepository.GetAsync(record => record.Id == productId);
            if (product == null)
            {
                throw new BadHttpRequestException("Record not found!", StatusCodes.Status404NotFound);
            }
            return _mapper.Map<ProductPublicDTO>(product);
        }

        public async Task<ProductPublicDTO> CreateProductAsync(ProductCreateDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            var category = await _categoryRepository.GetAsync(record => record.Id == productDto.CategoryId);
            if (category == null)
            {
                throw new BadHttpRequestException("Category doesn't exist", StatusCodes.Status404NotFound);
            }

            var productDb = await _productRepository.CreateAsync(product);
            return _mapper.Map<ProductPublicDTO>(productDb);
        }

        public async Task<ProductPublicDTO> UpdateProductAsync(int productId, ProductUpdateDTO productUpdate)
        {
            var product = await _productRepository.GetAsync(record => record.Id == productId, true);
            if (product == null)
            {
                throw new BadHttpRequestException("Record not found!", StatusCodes.Status404NotFound);
            }

            product.Name = productUpdate.Name;
            product.Description = productUpdate.Description;
            product.CategoryId = productUpdate.CategoryId;
            product.Images = productUpdate.Images;

            var updatedProduct = await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductPublicDTO>(updatedProduct);
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _productRepository.GetAsync(record => record.Id == productId, true);
            if (product == null)
            {
                throw new BadHttpRequestException("Record not found!", StatusCodes.Status404NotFound);
            }

            await _productRepository.RemoveAsync(product);
            return true;
        }
    }
}

