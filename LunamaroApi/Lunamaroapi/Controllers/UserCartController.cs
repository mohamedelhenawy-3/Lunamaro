using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Migrations;
using Lunamaroapi.Models;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lunamaroapi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserCartController : ControllerBase
    {
        private readonly IUserCart _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserCartController(IUserCart cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }

        [HttpGet("mycart")]
        public async Task<ActionResult<List<UserCartDTO>>> GetCarts()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var cartItems = await _cartService.GetCartItemsAsync(userId);
            return Ok(cartItems);
        }


        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();


            await _cartService.RemoveFromCartAsync(cartItemId);
            return Ok(new { message = "Item removed from cart successfully." });
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO request)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
                return Unauthorized();
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Invalid request.");
            }
            try
            {
                await _cartService.AddToCartAsync(request.UserId, request.ItemId, request.Quantity);
                return Ok("Item added to cart.");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost("update-quantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityDTO dto)
        {
            if (dto.NewQuantity <= 0 || dto.NewQuantity > 10)
                return BadRequest("Quantity must be between 1 and 10.");

            await _cartService.UpdateQuantityAsync(dto.CartItemId, dto.NewQuantity);

            return Ok(new { message = "Quantity updated successfully." });
        }

    }
}
