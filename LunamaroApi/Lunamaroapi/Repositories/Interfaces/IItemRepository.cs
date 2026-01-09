using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.Models;

namespace Lunamaroapi.Repositories.Interfaces
{
    public interface IItemRepository
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> GetSpecialItemsAsync();

        Task<Item?> GetItemByIdAsync(int id);
        Task CreateItemAsync(Item item);
        Task UpdateItemAsync(Item item, int id);
        Task DeleteItemAsync(int id);


        Task<IEnumerable<Item>> GetItemByCatId(int catId);
        Task<IEnumerable<Item>> ExploreItemMenu();
        Task<IEnumerable<Item>> ExplorePopularItems();
        Task<bool> Exists(int id);
    }
}
