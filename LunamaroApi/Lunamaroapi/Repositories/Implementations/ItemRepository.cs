using Lunamaroapi.Data;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Repositories.Implementations
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDBContext _context;

        public ItemRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task CreateItemAsync(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null) return;
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Items.AnyAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _context.Items
                .Include(c => c.Category) // Include category if needed
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateItemAsync(Item item, int id)
        {
            var existingItem = await _context.Items.FindAsync(id);
            if (existingItem == null)
                throw new ArgumentException("Item not found");

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Price = item.Price;
            existingItem.quantity = item.quantity;
            existingItem.CategoryId = item.CategoryId;
            existingItem.ImageUrl = item.ImageUrl;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Item>> GetItemByCatId(int catId)
        {
            return await _context.Items
                .Where(i => i.CategoryId == catId)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> ExploreItemMenu()
        {
            return await _context.Items
                .OrderByDescending(x => x.Id)
                .Take(5)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Item>> ExplorePopularItems()
        {
            var popularItems = await _context.OrderItems
                .GroupBy(oi => new { oi.ItemId, oi.Item.Name, oi.Item.Description, oi.Item.Price, oi.Item.ImageUrl })
                .Select(g => new Item
                {
                    Id = g.Key.ItemId,
                    Name = g.Key.Name,
                    Description = g.Key.Description,
                    Price = g.Key.Price,
                    ImageUrl = g.Key.ImageUrl
                })
                .OrderByDescending(x => _context.OrderItems.Where(o => o.ItemId == x.Id).Sum(o => o.Quantity))
                .Take(5)
                .ToListAsync();

            return popularItems;
        }

        public async Task<IEnumerable<Item>> GetSpecialItemsAsync()
        {

            return await _context.Items.Where(i => i.IsSpecial)
                .OrderBy(x=>x.Name)
                .Take(6).ToListAsync();
        
        }
    }
}
