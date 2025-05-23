using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lunamaroapi.Services
{

    public class ItemService : IItem
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ItemService(AppDBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
    
        }

        public async Task<ItemDTO> CreateItemAsync(ItemDTO itemdto)
        {
         
            var item = new Item
            {
                Name = itemdto.Name,
                Description = itemdto.Description,
                Price = itemdto.Price,
                CategoryId = itemdto.CategoryId
            };
            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            var savedItem = await _db.Items
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.Id == item.Id);

            return new ItemDTO
            {
                Name = itemdto.Name,
                Description = itemdto.Description,
                Price = itemdto.Price,
                CategoryId = itemdto.CategoryId
            
            };

        }

        public async  Task DeleteItemAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return;
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }

        public Task<bool> Exists(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ItemDTO>> GetAllItemsAsync()
        {
            var items = await _db.Items
              .Include(n => n.Category)  // Include Category if you need category data as well
              .ToListAsync();

            return items.Select(item => new ItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId
            });
        }

        public async Task<ItemDTO?> GetItemByIdAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return null;
            return new ItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = item.CategoryId
            };
        }

        public Task UpdateItemAsync(ItemDTO itemdto, int id)
        {
            throw new NotImplementedException();
        }
    }
}
