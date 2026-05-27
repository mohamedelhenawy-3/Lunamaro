using Lunamaroapi.Data;
using Lunamaroapi.Helper;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly ICategoryService _categoryService;
        private readonly IReview _reviewService;
        private readonly AppDBContext _db;

        public HomeController(IItemService itemService, ICategoryService categoryService,
            AppDBContext db, IReview reviewService)
        {
            _itemService = itemService;
            _categoryService = categoryService;
            _reviewService = reviewService;
            _db = db;
        }
        [HttpGet("home-data")]
        public async Task<IActionResult> GetHomeData()
        {
            var now = DateTime.UtcNow;

            // Run sequentially — EF Core DbContext is NOT thread-safe
            var weeklyDeals = await SafeExecute(async () => (object)await _db.WeeklyDeals
                .Include(w => w.Product)
                .Where(x => x.IsActive && x.ExpiryDate > now && x.Product != null)
                .Select(d => new {
                    d.Id,
                    d.ProductId,
                    d.DiscountPercentage,
                    d.ExpiryDate,
                    d.IsActive,
                    Product = new
                    {
                        d.Product.Id,
                        d.Product.Name,
                        d.Product.ImageUrl,
                        d.Product.Price,
                        FinalPrice = d.Product.Price - (d.Product.Price * d.DiscountPercentage / 100)
                    }
                }).ToListAsync(), "weeklyDeals");

            var discountTiers = await SafeExecute(async () => (object)await _db.DiscountTiers.Where(t => t.IsActive).ToListAsync(), "discountTiers");
            var addOnRewards = await SafeExecute(async () => (object)await _db.AddOnRewards.Include(r => r.FreeProduct).Where(r => r.IsActive).ToListAsync(), "addOnRewards");
            var popular = await SafeExecute(async () => (object)await _itemService.ExplorePopularItems(), "popular");
            var specialItems = await SafeExecute(async () => (object)await _itemService.GetSpecialItems(), "specialItems");
            var menuPreview = await SafeExecute(async () => (object)await _itemService.ExploreItemMenu(), "menuPreview");
            var categories = await SafeExecute(async () => (object)await _categoryService.GetAllAsync(), "categories");
            var latestReviews = await SafeExecute(async () => (object)await _reviewService.LatestReviews(), "latestReviews");

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Home data retrieved successfully",
                Data = new
                {
                    weeklyDeals = weeklyDeals.Data,
                    discountTiers = discountTiers.Data,
                    addOnRewards = addOnRewards.Data,
                    popular = popular.Data,
                    specialItems = specialItems.Data,
                    menuPreview = menuPreview.Data,
                    categories = categories.Data,
                    latestReviews = latestReviews.Data
                }
            });
        }

        private async Task<(bool Success, object? Data, string? Error, string Name)> SafeExecute(
            Func<Task<object>> action, string name)
        {
            try { return (true, await action(), null, name); }
            catch (Exception ex) { return (false, null, ex.InnerException?.Message ?? ex.Message, name); }
        }

    }
}