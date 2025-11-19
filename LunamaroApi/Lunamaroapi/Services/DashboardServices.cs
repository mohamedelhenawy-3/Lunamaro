using Lunamaroapi.Data;
using Lunamaroapi.DTOs.DashBoard;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Messaging;

namespace Lunamaroapi.Services
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
            var totalUsers = _db.Users.Count();
            var totalItemsMenu = _db.Items.Count();
            var totalrevenu =_db.UserOrderHeaders.Any()
    ? _db.UserOrderHeaders.Sum(x => x.TotalAmount)
    : 0;

            var totalorders = _db.OrderItems.Count();




            return new HeaderStatsDTO
            {
                TotalMenuItems = totalItemsMenu,
                TotalUsers = totalUsers,
                TotalRevenue = totalrevenu,
                TotalOrders = totalorders
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetDailyOrderSummary()
        {
            var today = DateTime.Today;
            var orders = await _db.UserOrderHeaders.Where(x => x.DateOfOrder.Date == today).GroupBy(x => x.OrderStatus).Select(g => new OrderSummaryDTO
            {
                Status = g.Key,
                count = g.Count()
            }).ToListAsync();
            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

            var result = allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);


                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });

            return result;
         
        }
        public async Task<RevenueDTO> GetDailyRevenueSummary()
        {
            var today = DateTime.Today;

            // Calculate revenue for today only
            var dailyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder.Date == today)
                .SumAsync(x => (double?)x.TotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = dailyRevenue
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetMnthlyOrderSummary()
        {
            var today = DateTime.Today;
            var orders = await _db.UserOrderHeaders
                     .Where(o => o.DateOfOrder.Month == today.Month &&
                    o.DateOfOrder.Year == today.Year)
                .GroupBy(x => x.OrderStatus).Select(g => new OrderSummaryDTO
                {
                    Status = g.Key,
                    count = g.Count()
                }).ToListAsync();


            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();
            var result = allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });

            return result;
        }

        public async Task<MonthlyOrdersDTO> GetMonthlyOrdersAsync()
        {
            var labels = new List<string>();
            var values = new List<int>();

            for (int month = 1; month <= 12; month++)
            {
                labels.Add(new DateTime(DateTime.Today.Year, month, 1).ToString("MMM"));

                int count = await _db.UserOrderHeaders
                    .Where(x => x.DateOfOrder.Month == month)
                    .CountAsync();

                values.Add(count);
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

            // Sum revenue for this month
            var monthlyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder.Month == today.Month &&
                            x.DateOfOrder.Year == today.Year)
                .SumAsync(x => (double?)x.TotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = monthlyRevenue
            };
           
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetOrderSunnary()
        {
            var orders = await _db.UserOrderHeaders.GroupBy(x => x.OrderStatus).Select(g => new OrderSummaryDTO
            {
                Status = g.Key,
                count = g.Count()
            }).ToListAsync();

            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();

            var result = allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);


                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });


            return result;




        }

        public async Task<ProductCategoryDTO> GetProductCategoriesAsync()
        {
            var labels = new List<string>();
            var values = new List<int>();

            var data = await _db.Items
                .Include(p => p.Category)
                .GroupBy(p => p.Category.Name)
                .Select(g => new
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            foreach (var item in data)
            {
                labels.Add(item.Category);
                values.Add(item.Count);
            }

            return new ProductCategoryDTO
            {
                Labels = labels,
                Values = values
            };
        }

        public async Task<IEnumerable<OrderRowDTO>> GetRecentOrders()
        {
            var orders =  await _db.UserOrderHeaders.OrderByDescending(x => x.DateOfOrder).Take(10).Select(x => new OrderRowDTO
            {
                OrderId = x.Id,
                Customer = x.User.FullName,
                Amount = x.TotalAmount,
                Status = x.OrderStatus,
                Date = x.DateOfOrder
            }).ToListAsync();

            return orders;


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
                    .Where(x => x.DateOfOrder.Date == date)
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0;

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
            var today = DateTime.Today;
            var startOfWeek = today.AddDays(-7); // last 7 days

            // Sum revenue from last 7 days
            var weeklyRevenue = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder.Date >= startOfWeek)
                .SumAsync(x => (double?)x.TotalAmount) ?? 0;

            return new RevenueDTO
            {
                TotalRevenue = weeklyRevenue
            };
        }

        public async Task<IEnumerable<OrderSummaryDTO>> GetWeeklyOrderSummary()
        {
            var startOfWeek = DateTime.Today.AddDays(-7);
            var orders = await _db.UserOrderHeaders
                .Where(x => x.DateOfOrder >= startOfWeek)
                .GroupBy(x => x.OrderStatus).Select(g => new OrderSummaryDTO
            {
                Status = g.Key,
                count = g.Count()
            }).ToListAsync();


            var allstatus = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().ToList();
            var result = allstatus.Select(status =>
            {
                var order = orders.FirstOrDefault(x => x.Status == status);
                return new OrderSummaryDTO
                {
                    Status = status,
                    count = order?.count ?? 0
                };
            });

            return result;
        }




    }
}
