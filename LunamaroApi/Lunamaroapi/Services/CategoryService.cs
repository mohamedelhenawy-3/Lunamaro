using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services
{
    public class CategoryService : ICategory
    {

        private readonly AppDBContext _dbcontext;
        public CategoryService(AppDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task AddAsync(CategoryDTO categoryDto)
        {
           
                var category = new Category
                {

                    Name = categoryDto.Name
                };
                await _dbcontext.AddAsync(category);
                await _dbcontext.SaveChangesAsync();
           
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null)
            {
                return;
            }

            _dbcontext.Categories.Remove(category);
            await _dbcontext.SaveChangesAsync();

        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _dbcontext.Categories.AnyAsync(c => c.Id == id);
        }


        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            return await _dbcontext.Categories
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    // Add other properties as needed
                })
                .ToListAsync();
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDTO
            {
                Name = category.Name
            };
        }


        public async Task UpdateAsync(int id, CategoryDTO categoryDto)
        {
            var category = await _dbcontext.Categories.FindAsync(id);
            if (category == null) return;

            category.Name = categoryDto.Name;
            _dbcontext.Categories.Update(category);
            await _dbcontext.SaveChangesAsync();
        }
    }
}
