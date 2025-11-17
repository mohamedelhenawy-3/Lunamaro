using Lunamaroapi.Services;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
            var data = await _IDashboard.getAllHeaderStats();
            return Ok(data);
        }
        [HttpGet("order-summary")]
        public async Task<IActionResult> GetOrderSummary()
        {
            var data = await _IDashboard.GetOrderSunnary();
            return Ok(data);
        }
        [HttpGet("order-summary/daily")]
        public async Task<IActionResult> GetDailySummary()
        {

            return Ok(await _IDashboard.GetDailyOrderSummary());
        }
        [HttpGet("order-summary/Weekly")]
        public async Task<IActionResult> getWeeklySummary()
        {

            return Ok(await _IDashboard.GetWeeklyOrderSummary());
        }
        [HttpGet("order-summary/Monthly")]
        public async Task<IActionResult> getMonthlySummary()
        {

            return Ok(await _IDashboard.GetMnthlyOrderSummary());
        }

        [HttpGet("DailyRevenue")]
        public async Task<IActionResult> getDailyRev()
        {

            return Ok(await _IDashboard.GetDailyRevenueSummary());
        }


    }
}
