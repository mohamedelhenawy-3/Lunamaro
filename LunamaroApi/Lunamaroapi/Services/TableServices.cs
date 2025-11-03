using Lunamaroapi.Data;
using Lunamaroapi.DTOs.TableDTO;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services
{
    public class TableServices : ITable
    {

        private readonly AppDBContext _db;


        public TableServices(AppDBContext db) {

            _db = db;
        }

        public async Task AddTableAsync(TablesDTO newTable)
        {
            var table = new Table
            {
                TableNumber = newTable.TableNumber,
                Capacity = newTable.Capacity,
                Location = newTable.Location,
                IsAvailable = newTable.IsAvailable  // ✅ directly map the enum
            };

            _db.Tables.Add(table);
            await _db.SaveChangesAsync();
        }


        public async  Task<IEnumerable<TablesDTO>> GetAllTablesAsync()
        {
            return await _db.Tables.Select(s => new TablesDTO
            { Id=s.Id,
                TableNumber = s.TableNumber,
                Capacity=s.Capacity,
                Location=s.Location,
                IsAvailable=s.IsAvailable

            }).ToListAsync();
        }

        public async Task<Table?> GetTableByIdAsync(int id)
        {
            return await _db.Tables.FindAsync(id);
        }

        public async Task SetAvailabilityAsync(int id, bool isAvailable)
        {
            var table = await _db.Tables.FindAsync(id);
            if (table != null)
            {
                table.IsAvailable = isAvailable ? TableStatus.Available : TableStatus.UnAvailable;
                await _db.SaveChangesAsync();
            }
        }


        public async Task UpdateTableAsync(Table table)
        {
            _db.Tables.Update(table);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateStatusAsync(UpdateTableStatusDTO dto, int id)
        {
            var table = await _db.Tables.FindAsync(id);

            if (table == null)
                throw new Exception("Reservation not found");

            table.IsAvailable = dto.IsAvailable;

            await _db.SaveChangesAsync(); // ✅ Use async version
        }

        public async Task DeleteTableAsync(int id)
        {
            var table = await _db.Tables.FindAsync(id);
            if (table != null)
            {
                _db.Tables.Remove(table);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UsersTableDto>> GetAllAvilableTable()
        {
            var availableTables = await _db.Tables.Where(x => x.IsAvailable == TableStatus.Available).ToListAsync();

            var result = availableTables.Select(t => new UsersTableDto
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                Location = t.Location
            }).ToList();

            return result;

        }
    }
}
