using Lunamaroapi.DTOs.DashBoard;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IDashboard
    {
        public Task<HeaderStatsDTO> getAllHeaderStats();
        public Task<IEnumerable<OrderSummaryDTO>> GetOrderSunnary();
        public  Task<IEnumerable<OrderSummaryDTO>> GetDailyOrderSummary();
        public Task<IEnumerable<OrderSummaryDTO>> GetWeeklyOrderSummary();
        public Task<IEnumerable<OrderSummaryDTO>> GetMnthlyOrderSummary();
        public Task<RevenueDTO> GetDailyRevenueSummary();
        public Task<RevenueDTO> GetMonthlyRevenueSummary();
        public Task<RevenueDTO> GetWeaklyRevenueSummary();

    }
}
