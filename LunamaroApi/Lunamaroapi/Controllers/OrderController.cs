using Lunamaroapi.DTOs;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderService;


        public OrderController(IOrder orderServices)
        {
            _orderService = orderServices;
        }

        // Controller Method
        [HttpGet("preview")]
        public async Task<IActionResult> GetOrderPreview()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            var result = await _orderService.GetOrderPerview(userId);

            if (result == null)
                return NotFound("No orders found for this user.");

            return Ok(result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderdto dto)
        {
            var result = await _orderService.OrderDone(dto);

            if (result == null)
                return BadRequest("Could not create order.");

            return Ok(result);
        }
        [HttpGet("success")]
        public async Task<IActionResult> PaymentSuccess([FromQuery] string sessionId)
        {
            var result = await _orderService.OrderSuccess(sessionId);

            if (!result)
                return BadRequest("Order not found or already processed.");

            return Ok("Payment Completed");
        }

        [HttpGet("history")]
        [Authorize] 
        public async Task<IActionResult> GetOrderHistory()
        {
            var history = await _orderService.UserOrderHistory();

            if (history == null || !history.Any())
                return Ok(new List<UserOrdersHistory>()); // return empty list

            return Ok(history);
        }
    }
}
