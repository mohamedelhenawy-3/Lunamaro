using Lunamaroapi.DTOs;
using Lunamaroapi.DTOs.Admin;
using Lunamaroapi.DTOs.Order;
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
        [HttpGet("AllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.ListOfOrders();

            if (orders == null || !orders.Any())
                return NotFound("No orders found.");

            return Ok(orders);
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
                return BadRequest("Cannot create order: pending order exists or user not logged in.");


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

        [HttpGet("history/{orderId}")]
        public async Task<IActionResult> OrderHistoryDetails(int orderId)
        {
            var result = await _orderService.UserOrderHistoryDetails(orderId);

            if (result == null)
                return NotFound("Order not found or not allowed");

            return Ok(result);
        }
        [HttpPost("{orderId}/update-status")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateStatusOrderDTO dto)
        {
           

            bool updated = await _orderService.UpdateStatusAsync(dto, orderId);

            if (!updated)
                return BadRequest("Failed to update order status");
            return Ok(new { message = "Order status updated successfully" });

        }


        [HttpGet("historyAd/{id}")]
        public async Task<IActionResult> GetOrderHistoryDetails(int id)
        {
            
            var result = await _orderService.OrderHistoryDetailsAd(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

    }
}
