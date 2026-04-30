using Lunamaroapi.Data;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lunamaroapi.Services.Implements
{
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDBContext _context; 
        private readonly IHttpContextAccessor _httpContextAccessor;

     public RecommendationService(AppDBContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<List<Item>> GetSuggestions()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return new List<Item>();

            var cartItems = await _context.UserCarts
                .Include(c => c.Item)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return new List<Item>();

            var cartItemIds = cartItems.Select(c => c.ItemId).ToList();

            var categoryIds = cartItems
                .Where(c => c.Item != null)
                .Select(c => c.Item.CategoryId)
                .ToList();

            // 1. RELATED FOOD EXPANSION
            var related = categoryIds
                .SelectMany(GetRelatedCategories)
                .Distinct();

            // 2. COMPLEMENTS
            var complements = GetComplementCategories();

            // 3. FINAL CATEGORY SET
            var finalCategories = related
                .Union(complements)
                .Distinct()
                .ToList();

            var suggestions = await _context.Items
                .Where(i => !cartItemIds.Contains(i.Id))
                .Where(i => finalCategories.Contains(i.CategoryId))
                .ToListAsync();

            return suggestions
                .OrderByDescending(i =>
                    categoryIds.Contains(i.CategoryId) ? 3 :
                    complements.Contains(i.CategoryId) ? 1 : 2
                    )
                    .ThenBy(i => i.Price)
                    .ToList();
        }



        private List<int> GetComplementCategories()
        {
            return new List<int>
             {
               1022, // Drinks
               1020, // Sides
               1021  // Desserts
             };
        }

        private List<int> GetRelatedCategories(int categoryId)
        {
            return categoryId switch
            {
                // FAST FOOD
                1010 => new List<int> { 1010, 1026, 1020, 1022, 1021 },
                1026 => new List<int> { 1026, 1010, 1020, 1022, 1021 },

                // MAIN FOOD
                1019 => new List<int> { 1019, 1012, 1025, 1018, 1017, 1020, 1022 },
                1012 => new List<int> { 1012, 1019, 1025, 1017, 1020, 1022 },
                1025 => new List<int> { 1025, 1019, 1012, 1017, 1020, 1022 },

                // LIGHT FOOD
                1018 => new List<int> { 1018, 1017, 1016, 1020, 1022, 1021 },
                1017 => new List<int> { 1017, 1018, 1016, 1020, 1022 },
                1016 => new List<int> { 1016, 1017, 1018, 1020, 1022 },

                // SWEETS
                1021 => new List<int> { 1021, 1027, 1022 },
                1027 => new List<int> { 1027, 1021, 1022 },

                // DRINKS
                1022 => new List<int> { 1022, 1020 },

                // BREAKFAST
                1028 => new List<int> { 1028, 1022, 1021 },

                _ => new List<int> { categoryId }
            };
        }









    }
}
