using Lunamaroapi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IItem
    {
        Task<IEnumerable<ItemDTO>> GetAllItemsAsync();
        Task<ItemDTO?> GetItemByIdAsync(int id);
        Task<ItemDTO> CreateItemAsync(ItemDTO itemdto);
        Task UpdateItemAsync(ItemDTO itemdto, int id);
        Task DeleteItemAsync(int id);


         Task<IEnumerable<ItemDTO>> GetItemByCatId(int catId);

        Task<bool> Exists(int id);
    }
}
