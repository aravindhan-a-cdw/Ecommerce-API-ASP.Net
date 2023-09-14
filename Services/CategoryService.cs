using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.Models.DTO.CategoryDTO;
using EcommerceAPI.Repository.IRepository;
using EcommerceAPI.Services.IServices;

namespace EcommerceAPI.Services
{
    public class CategoryService: ICategoryService
	{
        private readonly IRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IRepository<Category> categoryRepository, IMapper mapper)
		{
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        async public Task<List<CategoryPublicDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(Include: new() { "Products" });
            return _mapper.Map<List<CategoryPublicDTO>>(categories);
        }

        async public Task<CategoryPublicDTO> GetCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == categoryId, Include: new() { "Products" }) ?? throw new BadHttpRequestException("Record not found!", StatusCodes.Status404NotFound);
            return _mapper.Map<CategoryPublicDTO>(category);
        }

        async public Task<CategoryPublicDTO> CreateCategoryAsync(CategoryCreateDTO categoryCreate)
        {
            var existing = await _categoryRepository.GetAsync(record => record.Name == categoryCreate.Name);
            if (existing != null)
            {
                throw new BadHttpRequestException($"Category with name {categoryCreate.Name} already exists");
            }

            var category = _mapper.Map<Category>(categoryCreate);
            // Optionally, you can set other properties or perform additional validation here.

            var createdCategory = await _categoryRepository.CreateAsync(category);
            return _mapper.Map<CategoryPublicDTO>(createdCategory);
        }

        async public Task<CategoryPublicDTO> UpdateCategoryAsync(int categoryId, CategoryUpdateDTO updateDTO)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == categoryId) ?? throw new BadHttpRequestException("Record doesn't exist", StatusCodes.Status404NotFound);
            category.Name = updateDTO.Name;

            var updatedCategory = await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryPublicDTO>(updatedCategory);
        }

        async public Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepository.GetAsync(record => record.Id == categoryId) ?? throw new Exception("Record doesn't exist");
            await _categoryRepository.RemoveAsync(category);
            return true;
        }

    }
}

