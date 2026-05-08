
using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.Models;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace Lunamaroapi.Services.Implements
{
    public class ItemService : IItemService
    {

        private readonly IItemRepository _itemRepository;
        private readonly IImageServices _imageService;
        private readonly AppDBContext _db;
        private readonly IMemoryCache _cache;

        public ItemService(IItemRepository itemRepository, IMemoryCache cache, IImageServices imageService, AppDBContext db)
        {
            _itemRepository = itemRepository;
            _imageService = imageService;
            _db = db;
            _cache = cache;
        }



        public async Task<SuccessResponseDto> CreateItemAsync(ItemDTO itemdto)
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
            ClearMenuCache();

            return new SuccessResponseDto
            {
                Message = "Item Added Succefully"
            };
        }

        public async Task DeleteItemAsync(int id)
        {
            var item = await _itemRepository.GetItemByIdAsync(id);

            if (item == null)
                throw new ArgumentException("Item not found");

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                await _imageService.DeleteImage(item.ImageUrl);
            }

            await _itemRepository.DeleteItemAsync(id);
            ClearMenuCache();
        }

        public async Task<bool> Exists(int id)
        {
            return await _itemRepository.Exists(id);
        }

        public async Task<IEnumerable<ItemDTO>> GetAllItemsAsync()
        {
            return await _db.Items
                .AsNoTracking() // 🚀 huge performance boost
                .Select(i => new ItemDTO
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    quantity = i.quantity,
                    CategoryId = i.CategoryId,
                    ImageUrl = i.ImageUrl
                })
                .ToListAsync();
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
            return await _itemRepository.ExplorePopularItems();
        }

        public async Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId)
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
                if (!string.IsNullOrEmpty(existingItem.ImageUrl))
                {
                    await _imageService.DeleteImage(existingItem.ImageUrl);
                }

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
            ClearMenuCache();
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
                IsSpecial = i.IsSpecial
            });
        }

        public async Task<object> GetPaginatedMenuAsync(int page, int pageSize, int? categoryId = null)
        {
            string cacheKey = $"menu_p{page}_s{pageSize}_cat{categoryId ?? 0}";

            if (!_cache.TryGetValue(cacheKey, out object cachedData))
            {
                var query = _db.Items.AsNoTracking();

                if (categoryId.HasValue && categoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == categoryId.Value);
                }

                var totalCount = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new
                    {
                        i.Id,
                        i.Name,
                        i.Description,
                        i.Price,
                        i.ImageUrl,
                        i.quantity
                    })
                    .ToListAsync();

                cachedData = new
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                // 5. تخزين النتيجة في الكاش
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _cache.Set(cacheKey, cachedData, cacheOptions);
            }

            return cachedData;
        }
        public Task<object> GetPaginatedMenuAsync(int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<object> GetAdminItems(int page, int pageSize, int? categoryId = null, string? search = null)
        {
            string cacheKey = $"menu_p{page}_s{pageSize}_cat{categoryId ?? 0}_q{search ?? ""}";
            if (!_cache.TryGetValue(cacheKey, out object cachedData))
            {
                var query = _db.Items.AsNoTracking();
                if (categoryId.HasValue && categoryId > 0)
                    query = query.Where(x => x.CategoryId == categoryId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(x =>
                      x.Name.Contains(search) ||
                      x.Description.Contains(search));
                }


                var totalCount = await query.CountAsync();
                var items = await query
                    .OrderByDescending(x => x.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(i => new
                    {
                        i.Id,
                        i.Name,
                        i.Description,
                        i.Price,
                        i.ImageUrl,
                        i.quantity,
                    })
                    .ToListAsync();
                cachedData = new
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
                var ttl = string.IsNullOrWhiteSpace(search)
                         ? TimeSpan.FromMinutes(10)
                          : TimeSpan.FromMinutes(2);

                _cache.Set(cacheKey, cachedData,
                      new MemoryCacheEntryOptions()
                     .SetAbsoluteExpiration(ttl));
            }

                return cachedData;


            }

        private void ClearMenuCache()
        {
            if (_cache is MemoryCache memoryCache)
            {
                memoryCache.Clear();
            }
        }
    }
}

