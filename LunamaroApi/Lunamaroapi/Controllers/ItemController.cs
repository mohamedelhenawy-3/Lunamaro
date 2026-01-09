using FluentValidation;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Implements;
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
        private readonly IItemService _IItemService;


        public ItemController(IItemService itemsrvice)
        {
            _IItemService = itemsrvice;
        }
        [Authorize(Roles = "Admin")]

        [HttpPost("CreateItem")]
        [Consumes("multipart/form-data")]

        public async Task<ActionResult> CreateItem([FromForm] ItemDTO itemdto, [FromServices] IValidator<ItemDTO> validator)
        {
            await validator.ValidateAsync(itemdto, options =>options.IncludeRuleSets("Create"));

            return Ok(await _IItemService.CreateItemAsync(itemdto));

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


        [HttpGet("SpecialItems")]
        public async Task<ActionResult<IEnumerable<ItemDTO>>> GetSpecialItems()
        {
            var items = await _IItemService.GetSpecialItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDTO>> GetItemById(int id)
        {
            var item = await _IItemService.GetItemByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }



        [Authorize(Roles = "Admin")]

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItem([FromForm] UpdateItemDTO itemdto, int id)
        {

            await _IItemService.UpdateItemAsync(itemdto, id);
            return NoContent();

        }

        [Authorize(Roles = "Admin")]

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
