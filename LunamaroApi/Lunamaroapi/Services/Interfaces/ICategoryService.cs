using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Category;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<CreateCategoryDTO> AddAsync(CreateCategoryDTO categoryDto);
        Task UpdateAsync(int id, CategoryDTO categoryDto);
        Task<string> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
