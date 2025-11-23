using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Lunamaroapi.Services
{

    public class ItemService : IItem
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IImageServices _imageService;


        public ItemService(AppDBContext db, IHttpContextAccessor httpContextAccessor, IImageServices imageService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _imageService = imageService;
        }

        public async Task<ItemDTO> CreateItemAsync(ItemDTO itemdto)
        {
            if (itemdto.File == null || itemdto.File.Length == 0) throw new ArgumentException("Imge file required");
            var imageUrl = await _imageService.UploadImage(itemdto.File);

            var item = new Item
            {
                Name = itemdto.Name,
                Description = itemdto.Description,
                Price = itemdto.Price,
                quantity=itemdto.quantity,
                CategoryId = itemdto.CategoryId,
                ImageUrl = imageUrl
            };
            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            var savedItem = await _db.Items
            .Include(n => n.Category)
            .FirstOrDefaultAsync(n => n.Id == item.Id);

            return new ItemDTO
            { Id=item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                quantity=item.quantity,
                CategoryId = item.CategoryId,
               ImageUrl = item.ImageUrl

            };

        }

        public async  Task DeleteItemAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return;
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _db.Items.AnyAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<ItemDTO>> GetAllItemsAsync()
        {
            var items = await _db.Items
              .Include(n => n.Category)  // Include Category if you need category data as well
              .ToListAsync();

            return items.Select(item => new ItemDTO
            {
                Id=item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                quantity=item.quantity,
                CategoryId = item.CategoryId,
                ImageUrl = item.ImageUrl
        

            });
        }

 



        public async Task<ReturnedItemDTO?> GetItemByIdAsync(int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null) return null;
            return new ReturnedItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                quantity=item.quantity,
                ImageUrl=item.ImageUrl,
               CategoryId=item.CategoryId
            };
        }

        public async Task UpdateItemAsync(ItemDTO itemdto, int id)
        {
            var item = await _db.Items.FindAsync(id);
            if (item == null)
                throw new ArgumentException("Item not found");

        
                var imageUrl = await _imageService.UploadImage(itemdto.File);
                item.ImageUrl = imageUrl;
         
            // Update other fields
            item.Name = itemdto.Name;
            item.Price = itemdto.Price;
            item.Description = itemdto.Description;
            item.CategoryId = itemdto.CategoryId;

            await _db.SaveChangesAsync();
        }



        public async  Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId)
        {
            var items = await _db.Items
               .Where(i => i.CategoryId == catId)
               .Select(item => new ItemDTO
               {
                   Id = item.Id,
                   Name = item.Name,
                   Description = item.Description,
                   Price = item.Price,
                   quantity=item.quantity,
                   CategoryId = item.CategoryId,
                   ImageUrl = item.ImageUrl
               })
               .ToListAsync();

            return items;
        }



        public async Task<IEnumerable<ExplorePopItems>> ExploreItemMenu()
        {
            var items = await _db.Items
                .OrderByDescending(x => x.Id)
                .Take(5)
                .Select(x => new ExplorePopItems
                {
                   
                    Name = x.Name,
                    Description=x.Description,
                    Price = x.Price,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync();

            return items;
        }

        public async Task<IEnumerable<ExplorePopItems>> ExplorePopularItems()
        {
            var popularItems = await _db.OrderItems
       .GroupBy(oi => new { oi.ItemId, oi.Item.Name, oi.Item.Description, oi.Item.Price, oi.Item.ImageUrl })
       .Select(g => new
       {
           Name = g.Key.Name,
           Description = g.Key.Description,
           Price = g.Key.Price,
           ImageUrl = g.Key.ImageUrl,
           TotalOrdered = g.Sum(x => x.Quantity)
       })
       .OrderByDescending(x => x.TotalOrdered)
       .Take(5)
       .Select(x => new ExplorePopItems
       {
           Name = x.Name,
           Description = x.Description,
           Price = x.Price,
           ImageUrl = x.ImageUrl
       })
       .ToListAsync();

            return popularItems;
        }

     
    }
}
