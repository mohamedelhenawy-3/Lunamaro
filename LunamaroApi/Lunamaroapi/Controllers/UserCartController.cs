using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.UserCart;
using Lunamaroapi.Models;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRecommendationService _recommendationService;

        public UserCartController(IUserCart cartService, UserManager<ApplicationUser> userManager, IRecommendationService recommendationService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _recommendationService = recommendationService;
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _recommendationService.GetSuggestions();

            return Ok(result);
        }
        [HttpGet("suggestions2")]
        public async Task<IActionResult> GetSuggestionsNew()
        {

            var result = await _recommendationService.GetSuggestionsV2();

            return Ok(result);
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
        [HttpGet("v2")]
        public async Task<IActionResult> GetCartV2()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartService.GetCartItemsV2(userId);

            return Ok(result);
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
        [HttpGet("count")]
        public async Task<IActionResult> GetCartItemCount()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var count = await _cartService.GetItemsCartCount(userId);
            return Ok(count);
        }


        [HttpPost("AddtoCartv2")]
        public async Task<IActionResult> AddToCartV2([FromBody] AddToCartDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not logged in");

            await _cartService.AddToCartV2(dto, userId);

            return Ok(new
            {
                message = "Item added to cart successfully"
            });
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO request)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." });

            if (request == null || request.Quantity <= 0)
                return BadRequest(new { message = "Invalid request data." });

            try
            {
                await _cartService.AddToCartAsync(userId, request.ItemId, request.Quantity);
                return Ok(new { message = "Item successfully added to cart." }); // ✅ Valid JSON
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // e.g., "Item not found."
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
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
        [HttpPut("UpdateAddOns")]
        [Authorize]
        public async Task<IActionResult> UpdateCartAddOns([FromBody] UpdateCartAddOnsDto dto)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "User not authenticated." }); await _cartService.UpdateCartAddOnsAsync(dto.UserCartId, dto.AddOnIds, userId);
            return Ok();
        }




    }
}
