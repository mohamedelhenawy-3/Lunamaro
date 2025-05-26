using Lunamaroapi.DTOs;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class ItemController : ControllerBase
    {
        private readonly IItem _IItemService;


        public ItemController(IItem itemsrvice)
        {
            _IItemService = itemsrvice;
        }

        [HttpPost("CreateItem")]
        public async Task<ActionResult> CreateItem([FromBody] ItemDTO itemdto)
        {
            await _IItemService.CreateItemAsync(itemdto);
            return StatusCode(201);

        }
        //ActionResult return a status code also
        [HttpGet("AllNote")]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetAllItems()
        {
            var items = await _IItemService.GetAllItemsAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemById(int id)
        {
            var item = await _IItemService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNote(ItemDTO itemdto, int id)
        {
            var exists = await _IItemService.Exists(id);

            if (!exists)
                return NotFound();

            await _IItemService.UpdateItemAsync(itemdto, id);
            return NoContent();

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var exists = await _IItemService.Exists(id);
            if (!exists)
                return NotFound();

            await _IItemService.DeleteItemAsync(id);
            return NoContent();
        }



    }
}
