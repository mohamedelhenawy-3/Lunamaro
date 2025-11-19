using Lunamaroapi.DTOs.DashBoard;
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

        // ---------------------------------------------------------
        // HEADER STATS (Total Users, Revenue, Orders, Menu Items)
        // ---------------------------------------------------------
        [HttpGet("header-stats")]
        public async Task<IActionResult> GetHeaderStats()
        {
            return Ok(await _IDashboard.getAllHeaderStats());
        }

        // ---------------------------------------------------------
        // FULL ORDER SUMMARY (All-Time)
        // ---------------------------------------------------------
        [HttpGet("order-summary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            return Ok(await _IDashboard.GetOrderSunnary());
        }

        // ---------------------------------------------------------
        // DAILY ORDER SUMMARY
        // ---------------------------------------------------------
        [HttpGet("order-summary/daily")]
        public async Task<IActionResult> GetDailySummary()
        {
            return Ok(await _IDashboard.GetDailyOrderSummary());
        }

        // ---------------------------------------------------------
        // WEEKLY ORDER SUMMARY
        // ---------------------------------------------------------
        [HttpGet("order-summary/weekly")]
        public async Task<IActionResult> GetWeeklySummary()
        {
            return Ok(await _IDashboard.GetWeeklyOrderSummary());
        }

        // ---------------------------------------------------------
        // MONTHLY ORDER SUMMARY
        // ---------------------------------------------------------
        [HttpGet("order-summary/monthly")]
        public async Task<IActionResult> GetMonthlySummary()
        {
            return Ok(await _IDashboard.GetMnthlyOrderSummary());
        }

        // ---------------------------------------------------------
        // DAILY REVENUE
        // ---------------------------------------------------------
        [HttpGet("revenue/daily")]
        public async Task<IActionResult> GetDailyRevenue()
        {
            return Ok(await _IDashboard.GetDailyRevenueSummary());
        }

        // ---------------------------------------------------------
        // WEEKLY REVENUE
        // ---------------------------------------------------------
        [HttpGet("revenue/weekly")]
        public async Task<IActionResult> GetWeeklyRevenue()
        {
            return Ok(await _IDashboard.GetWeaklyRevenueSummary());
        }

        // ---------------------------------------------------------
        // MONTHLY REVENUE
        // ---------------------------------------------------------
        [HttpGet("revenue/monthly")]
        public async Task<IActionResult> GetMonthlyRevenue()
        {
            return Ok(await _IDashboard.GetMonthlyRevenueSummary());
        }

        // ---------------------------------------------------------
        // CHART: REVENUE LAST 7 DAYS (Line Chart)
        // ---------------------------------------------------------
        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart()
        {
            return Ok(await _IDashboard.GetRevenueLast7DaysAsync());
        }

        // ---------------------------------------------------------
        // CHART: MONTHLY ORDERS (Bar Chart)
        // ---------------------------------------------------------
        [HttpGet("monthly-orders-chart")]
        public async Task<IActionResult> GetMonthlyOrdersChart()
        {
            return Ok(await _IDashboard.GetMonthlyOrdersAsync());
        }

        // ---------------------------------------------------------
        // CHART: PRODUCT CATEGORIES (Pie Chart)
        // ---------------------------------------------------------
        [HttpGet("product-categories-chart")]
        public async Task<IActionResult> GetProductCategoriesChart()
        {
            return Ok(await _IDashboard.GetProductCategoriesAsync());
        }

        // ---------------------------------------------------------
        // RECENT ORDERS (Last 10 Orders)
        // ---------------------------------------------------------
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            return Ok(await _IDashboard.GetRecentOrders());
        }
    }
}
