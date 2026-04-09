using Lunamaroapi.DTOs.DashBoard;
using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboard _IDashboard;

        public DashboardController(IDashboard IDashboard)
        {
            _IDashboard = IDashboard;
        }

   
        [HttpGet("header-stats")]
        public async Task<IActionResult> GetHeaderStats()
        {
            return Ok(await _IDashboard.getAllHeaderStats());
        }

        [HttpGet("order-summary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            return Ok(await _IDashboard.GetOrderSunnary());
        }

 
        [HttpGet("order-summary/daily")]
        public async Task<IActionResult> GetDailySummary()
        {
            return Ok(await _IDashboard.GetDailyOrderSummary());
        }


        [HttpGet("order-summary/weekly")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            return Ok(await _IDashboard.GetWeeklyOrderSummary());
        }

   
        [HttpGet("order-summary/monthly")]
        public async Task<IActionResult> GetMonthlySummary()
        {
            return Ok(await _IDashboard.GetMnthlyOrderSummary());
        }

   
        [HttpGet("revenue/daily")]
        public async Task<IActionResult> GetDailyRevenue()
        {
            return Ok(await _IDashboard.GetDailyRevenueSummary());
        }

      
        [HttpGet("revenue/weekly")]
        public async Task<IActionResult> GetWeeklyRevenue()
        {
            return Ok(await _IDashboard.GetWeaklyRevenueSummary());
        }

      
        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            return Ok(await _IDashboard.GetMonthlyRevenueSummary());
        }

        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart()
        {
            return Ok(await _IDashboard.GetRevenueLast7DaysAsync());
        }

        [HttpGet("monthly-orders-chart")]
        public async Task<IActionResult> GetMonthlyOrders()
        {
            try
            {
                var data = await _IDashboard.GetMonthlyOrdersAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("product-categories-chart")]
        public async Task<IActionResult> GetProductCategoriesChart()
        {
            return Ok(await _IDashboard.GetProductCategoriesAsync());
        }

   
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            return Ok(await _IDashboard.GetRecentOrders());
        }
    }
}
