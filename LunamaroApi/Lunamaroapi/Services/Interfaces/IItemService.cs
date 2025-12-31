using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Item;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<ItemDTO>> GetAllItemsAsync();
        Task<ReturnedItemDTO?> GetItemByIdAsync(int id);
        Task<ItemDTO> CreateItemAsync(ItemDTO itemdto);
        Task UpdateItemAsync(UpdateItemDTO itemdto, int id);
        Task DeleteItemAsync(int id);
        Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId);
        Task<IEnumerable<ExplorePopItems>> ExploreItemMenu();
        Task<IEnumerable<ExplorePopItems>> ExplorePopularItems();
        Task<bool> Exists(int id);
    }
}
