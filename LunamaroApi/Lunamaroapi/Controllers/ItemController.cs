using Lunamaroapi.DTOs;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]

    public class ItemController : ControllerBase
    {
        private readonly IItem _IItemService;


        public ItemController(IItem itemsrvice)
        {
            _IItemService = itemsrvice;
        }

        [HttpPost("CreateItem")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> CreateItem([FromForm] ItemDTO itemdto)
        {
            //Console.WriteLine($"File Received: {itemdto.File?.FileName}, Length: {itemdto.File?.Length}");

            var result = await _IItemService.CreateItemAsync(itemdto);
            return Ok(result);

        }
        [HttpGet("GetItemsByCategory/{catId}")]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetItemsByCategory(int catId)
        {
            var items = await _IItemService.GetItemByCatId(catId);
            return Ok(items);
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
        public async Task<ActionResult> UpdateItem([FromForm] ItemDTO itemdto, int id)
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




        // GET: api/items/menu-preview
        [HttpGet("menu-preview")]
        public async Task<IActionResult> GetMenuPreview()
        {
            // Call your existing service/repository method
            var items = await _IItemService.ExploreItemMenu(); 

            return Ok(items);
        }
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularItems()
        {
            var items = await _IItemService.ExplorePopularItems();
            return Ok(items);
        }



  
    }
}
