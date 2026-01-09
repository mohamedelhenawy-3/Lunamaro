
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Lunamaroapi.Services.Implements
{
    public class ItemService : IItemService
    {

        private readonly IItemRepository _itemRepository;
        private readonly IImageServices _imageService;

        public ItemService(IItemRepository itemRepository, IImageServices imageService)
        {
            _itemRepository = itemRepository;
            _imageService = imageService;
        }

        public async Task<ItemDTO> CreateItemAsync(ItemDTO itemdto)
        {
            if (itemdto.File == null || itemdto.File.Length == 0)
                throw new ArgumentException("Image file required");

            var imageUrl = await _imageService.UploadImage(itemdto.File);

            var item = new Item
            {
                Name = itemdto.Name,
                Description = itemdto.Description,
                Price = itemdto.Price,
                quantity = itemdto.quantity,
                CategoryId = itemdto.CategoryId,
                ImageUrl = imageUrl
            };

            await _itemRepository.CreateItemAsync(item);

            return new ItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                quantity = item.quantity,
                CategoryId = item.CategoryId,
                ImageUrl = item.ImageUrl
            };
        }

        public async Task DeleteItemAsync(int id)
        {
            await _itemRepository.DeleteItemAsync(id);
        }

        public async Task<bool> Exists(int id)
        {
            return await _itemRepository.Exists(id);
        }

        public async Task<IEnumerable<ItemDTO>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllItemsAsync();
            return items.Select(i => new ItemDTO
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                quantity = i.quantity,
                CategoryId = i.CategoryId,
                ImageUrl = i.ImageUrl
            });
        }

        public async Task<ReturnedItemDTO?> GetItemByIdAsync(int id)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);
            if (item == null) return null;

            return new ReturnedItemDTO
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                quantity = item.quantity,
                CategoryId = item.CategoryId,
                ImageUrl = item.ImageUrl
            };
        }
        public async Task<IEnumerable<ExplorePopItems>> ExploreItemMenu()
        {
            var items = await _itemRepository.ExploreItemMenu();
            return items.Select(i => new ExplorePopItems
            {
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl
            });
        }

        public async Task<IEnumerable<ExplorePopItems>> ExplorePopularItems()
        {
            var items = await _itemRepository.ExplorePopularItems();
            return items.Select(i => new ExplorePopItems
            {
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl
            });
        }

        public async  Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId)
        {
            var items = await _itemRepository.GetItemByCatId(catId);
            return items.Select(i => new ItemDTO
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                quantity = i.quantity,
                CategoryId = i.CategoryId,
                ImageUrl = i.ImageUrl
            });
        }



        public async Task UpdateItemAsync(UpdateItemDTO itemdto, int id)
        {
            var existingItem = await _itemRepository.GetItemByIdAsync(id);
            if (existingItem == null)
                throw new ArgumentException("Item not found");

            if (itemdto.File != null && itemdto.File.Length > 0)
            {
                var imageUrl = await _imageService.UploadImage(itemdto.File);
                existingItem.ImageUrl = imageUrl;
            }

            existingItem.Name = itemdto.Name;
            existingItem.Description = itemdto.Description;
            existingItem.Price = itemdto.Price;
            existingItem.quantity = itemdto.quantity;
            existingItem.CategoryId = itemdto.CategoryId;
            existingItem.IsSpecial = itemdto.IsSpecial;

            await _itemRepository.UpdateItemAsync(existingItem, id);
        }

        public async Task<IEnumerable<SpecialItems>> GetSpecialItems()
        {
            var items = await _itemRepository.GetSpecialItemsAsync();
            return items.Select(i => new SpecialItems
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                quantity = i.quantity,
                CategoryId = i.CategoryId,
                ImageUrl = i.ImageUrl,
                IsSpecial=i.IsSpecial
            });
        }
    }
}
