using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Category;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lunamaroapi.Services.Implements
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<CreateCategoryDTO> AddAsync(CreateCategoryDTO categoryDto)
        {
            try
            {
                var category = new Category
                {
                    Name = categoryDto.Name
                };

                var created = await _repository.AddAsync(category);

                _logger.LogInformation("Category added successfully. Id: {Id}, Name: {Name}", created.Id, created.Name);

                return new CreateCategoryDTO
                {
                    Id = created.Id,
                    Name = created.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category: {Name}", categoryDto.Name);
                throw; // rethrow to be handled by GlobalExceptionMiddleware
            }
        }

        public async Task<string> DeleteAsync(int id)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Delete failed: Category not found. Id: {Id}", id);
                    return "Category not found";
                }

                await _repository.DeleteAsync(category);
                _logger.LogInformation("Category deleted successfully. Id: {Id}", id);

                return "Category deleted successfully";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category. Id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _repository.GetAllAsync();

            _logger.LogInformation("Retrieved {Count} categories", categories.Count());

            return categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found. Id: {Id}", id);
                return null;
            }

            _logger.LogInformation("Category retrieved. Id: {Id}, Name: {Name}", category.Id, category.Name);

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task UpdateAsync(int id, CategoryDTO categoryDto)
        {
            try
            {
                var category = await _repository.GetByIdAsync(id);
                if (category == null)
                {
                    _logger.LogWarning("Update failed: Category not found. Id: {Id}", id);
                    throw new Exception("Category not found");
                }

                category.Name = categoryDto.Name;
                await _repository.UpdateAsync(category);

                _logger.LogInformation("Category updated successfully. Id: {Id}, Name: {Name}", id, category.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category. Id: {Id}", id);
                throw;
            }
        }
    }
}
