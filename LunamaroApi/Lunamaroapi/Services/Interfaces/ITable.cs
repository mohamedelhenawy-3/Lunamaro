using Lunamaroapi.DTOs.TableDTO;
using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface ITable
    {

        Task<IEnumerable<TablesDTO>> GetAllTablesAsync();
        Task<Table?> GetTableByIdAsync(int id);
        Task AddTableAsync(TablesDTO newTable);

        Task UpdateTableAsync(Table table);
        Task DeleteTableAsync(int id);
        Task SetAvailabilityAsync(int id, bool isAvailable);



        Task UpdateStatusAsync(UpdateTableStatusDTO dto, int id);

        Task<IEnumerable<UsersTableDto>> GetAllAvilableTable();

    }
}
