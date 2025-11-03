using Lunamaroapi.DTOs.TableDTO;
using Lunamaroapi.Models;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class TableController : ControllerBase
    {

        private readonly ITable _tablerepo;
        public TableController(ITable repo)
        {
            _tablerepo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> getAllTables()
        {
            var Tables = await _tablerepo.GetAllTablesAsync();
            return Ok(Tables);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetTable(int id)
        {
            var table = await _tablerepo.GetTableByIdAsync(id);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPost]
        public async Task<IActionResult> AddTable([FromBody] TablesDTO table)
        {
            await _tablerepo.AddTableAsync(table);
            return Ok(table);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(int id, [FromBody] Table updated)
        {

            var existing =await _tablerepo.GetTableByIdAsync(id);
            if (existing == null) return NotFound();


            existing.TableNumber = updated.TableNumber;
            existing.Capacity = updated.Capacity;
            existing.Location = updated.Location;
            existing.IsAvailable = updated.IsAvailable;

            await _tablerepo.UpdateTableAsync(existing);
            return Ok(existing);


        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            await _tablerepo.DeleteTableAsync(id);
            return Ok();
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTableStatusDTO dto)
        {
            await _tablerepo.UpdateStatusAsync(dto, id);
            return Ok(new { message = "Status updated successfully" });
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAllAvailableTables()
        {
            var tables = await _tablerepo.GetAllAvilableTable();
            return Ok(tables);
        }


    }
}
