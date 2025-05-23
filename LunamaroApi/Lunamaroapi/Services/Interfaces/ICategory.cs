using Lunamaroapi.DTOs;

namespace Lunamaroapi.Services.Interfaces
{
    public interface ICategory
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task AddAsync(CategoryDTO categoryDto);
        Task UpdateAsync(int id, CategoryDTO categoryDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
