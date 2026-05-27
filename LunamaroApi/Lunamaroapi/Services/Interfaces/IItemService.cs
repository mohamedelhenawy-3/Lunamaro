using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.AuthResponse;
using Lunamaroapi.DTOs.Item;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IItemService
    {


        Task<object> GetPaginatedMenuAsync(int page, int pageSize, int? categoryId = null);
        Task<object> GetAdminItems(int page, int pageSize, int? categoryId = null, string? search = null);
        




         Task<IEnumerable<ItemDTO>> GetAllItemsAsync();
        Task<IEnumerable<SpecialItems>> GetSpecialItems();
        Task<ItemDTO?> GetItemByIdAsync(int id);
        Task<SuccessResponseDto> CreateItemAsync(ItemDTO itemdto);
        Task UpdateItemAsync(UpdateItemDTO itemdto, int id);
        Task DeleteItemAsync(int id);
        Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId);
        Task<IEnumerable<ExplorePopItems>> ExploreItemMenu();
        Task<object> GetPaginatedMenuAsync(int page, int pageSize);
        Task<IEnumerable<ExplorePopItems>> ExplorePopularItems();
        Task<bool> Exists(int id);
    }
}
