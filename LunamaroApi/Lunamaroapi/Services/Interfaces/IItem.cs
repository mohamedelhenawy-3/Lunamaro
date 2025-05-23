using Lunamaroapi.DTOs;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IItem
    {
        Task<IEnumerable<ItemDTO>> GetAllItemsAsync();
        Task<ItemDTO?> GetItemByIdAsync(int id);
        Task<ItemDTO> CreateItemAsync(ItemDTO itemdto);
        Task UpdateItemAsync(ItemDTO itemdto, int id);
        Task DeleteItemAsync(int id);

        Task<bool> Exists(int id);
    }
}
