using EcommerceAPI.Models.DTO.CategoryDTO;

namespace EcommerceAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task<List<CategoryPublicDTO>> GetAllCategoriesAsync();
        Task<CategoryPublicDTO> GetCategoryAsync(int categoryId);
        Task<CategoryPublicDTO> CreateCategoryAsync(CategoryCreateDTO categoryCreate);
        Task<CategoryPublicDTO> UpdateCategoryAsync(int categoryId, CategoryUpdateDTO updateDTO);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}

