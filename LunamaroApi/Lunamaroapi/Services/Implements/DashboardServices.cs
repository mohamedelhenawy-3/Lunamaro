using Lunamaroapi.Data;
using Lunamaroapi.DTOs.DashBoard;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Services.Implements
{
    public class DashboardServices : IDashboard
    {
        private readonly AppDBContext _db;

        public DashboardServices(AppDBContext datbase)
        {
            _db = datbase;
        }

        public async Task<HeaderStatsDTO> getAllHeaderStats()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalItemsMenu = await _db.Items.CountAsync();

            var totalrevenu = await _db.UserOrderHeaders
                .SumAsync(x => (double?)x.FinalTotalAmount) ?? 0;

            var totalorders = await _db.OrderItems.CountAsync();

            return new HeaderStatsDTO
            {
                TotalMenuItems = totalItemsMenu,
                TotalUsers = totalUsers,
                TotalRevenue =(decimal) totalrevenu,
                TotalOrders = totalorders
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetDailyOrderSummary()
        {
            var today = DateTime.Today;

            var orders = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder >= today && x.DateOfOrder < today.AddDays(1))
                .GroupBy(x => x.OrderStatus)
                .Select(g => new OrderSummaryDTO
                {
                    Status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

            return allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });
        }

        public async Task<RevenueDTO> GetDailyRevenueSummary()
        {
            var today = DateTime.Today;

            var dailyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder >= today && x.DateOfOrder < today.AddDays(1))
                .SumAsync(x => (double?)x.FinalTotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = dailyRevenue
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetMnthlyOrderSummary()
        {
            var today = DateTime.Today;

            var orders = await _db.UserOrderHeaders
                .Where(o => o.DateOfOrder.Year == today.Year &&
                            o.DateOfOrder.Month == today.Month)
                .GroupBy(x => x.OrderStatus)
                .Select(g => new OrderSummaryDTO
                {
                    Status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

            return allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });
        }

        // 🔥 FIXED METHOD (MAIN ISSUE)
        public async Task<MonthlyOrdersDTO> GetMonthlyOrdersAsync()
        {
            var currentYear = DateTime.Today.Year;

            var data = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder != null && x.DateOfOrder.Year == currentYear)
                .GroupBy(x => x.DateOfOrder.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var labels = new List<string>();
            var values = new List<int>();

            for (int month = 1; month <= 12; month++)
            {
                labels.Add(new DateTime(currentYear, month, 1).ToString("MMM"));

                var item = data.FirstOrDefault(x => x.Month == month);
                values.Add(item?.Count ?? 0);
            }

            return new MonthlyOrdersDTO
            {
                Labels = labels,
                Values = values
            };
        }

        public async Task<RevenueDTO> GetMonthlyRevenueSummary()
        {
            var today = DateTime.Today;

            var monthlyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder.Year == today.Year &&
                            x.DateOfOrder.Month == today.Month)
                .SumAsync(x => (double?)x.FinalTotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = monthlyRevenue
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetOrderSunnary()
        {
            var orders = await _db.UserOrderHeaders
                .GroupBy(x => x.OrderStatus)
                .Select(g => new OrderSummaryDTO
                {
                    Status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

            return allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });
        }

        public async Task<ProductCategoryDTO> GetProductCategoriesAsync()
        {
            var data = await _db.Items
                .Include(p => p.Category)
                .GroupBy(p => p.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            return new ProductCategoryDTO
            {
                Labels = data.Select(x => x.Category).ToList(),
                Values = data.Select(x => x.Count).ToList()
            };
        }

        public async Task<IEnumerable<OrderRowDTO>> GetRecentOrders()
        {
            return await _db.UserOrderHeaders
                .OrderByDescending(x => x.DateOfOrder)
                .Take(10)
                .Select(x => new OrderRowDTO
                {
                    OrderId = x.Id,
                    Customer = x.User.FullName,
                    Amount = x.FinalTotalAmount,
                    Status = x.OrderStatus,
                    Date = x.DateOfOrder
                })
                .ToListAsync();
        }

        public async Task<RevenueChartDTO> GetRevenueLast7DaysAsync()
        {
            var labels = new List<string>();
            var values = new List<decimal>();

            for (int i = 6; i >= 0; i--)
            {
                var date = DateTime.Today.AddDays(-i);

                labels.Add(date.ToString("ddd"));

                var total = await _db.UserOrderHeaders
                    .Where(x => x.DateOfOrder >= date && x.DateOfOrder < date.AddDays(1))
                    .SumAsync(x => (decimal?)x.FinalTotalAmount) ?? 0;

                values.Add(total);
            }

            return new RevenueChartDTO
            {
                Labels = labels,
                Values = values
            };
        }

        public async Task<RevenueDTO> GetWeaklyRevenueSummary()
        {
            var startOfWeek = DateTime.Today.AddDays(-6);

            var weeklyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder >= startOfWeek)
                .SumAsync(x => (double?)x.FinalTotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = weeklyRevenue
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetWeeklyOrderSummary()
        {
            var startOfWeek = DateTime.Today.AddDays(-6);

            var orders = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder >= startOfWeek)
                .GroupBy(x => x.OrderStatus)
                .Select(g => new OrderSummaryDTO
                {
                    Status = g.Key,
                    count = g.Count()
                })
                .ToListAsync();

            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>();

            return allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });
        }
    }
}