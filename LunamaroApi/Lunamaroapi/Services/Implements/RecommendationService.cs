using Lunamaroapi.Data;
using Lunamaroapi.Models;
using Lunamaroapi.Models.CategoryEnums;
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
            return _httpContextAccessor.HttpContext?
                .User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?
                .Value;
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
            var relationships = await _context.categoryRelationships
                .Where(x => categoryIds.Contains(x.CategoryId)).ToListAsync();

            var additionalCategories = relationships.Where(r => r.Type == RelationType.Additionl)
                                                    .Select(r => r.RelatedCategoryId).Distinct().ToList();
            var complementCategories = relationships.Where(r => r.Type == RelationType.Complement)
                                                    .Select(r => r.RelatedCategoryId).Distinct().ToList();
            var relatedCategories = relationships.Where(r => r.Type == RelationType.Related)
                                                    .Select(r => r.RelatedCategoryId).Distinct().ToList();


            var allCategories = additionalCategories
                .Union(complementCategories)
                .Union(relatedCategories)
                .Distinct()
                .ToList();

            var suggestions = await _context.Items
                 .Where(i => !cartItemIds.Contains(i.Id))
                 .Where(i => allCategories.Contains(i.CategoryId))
                 .ToListAsync();

            // 5. Sort: addons first, then complements, then related
            return suggestions
                .OrderByDescending(i =>
                    additionalCategories.Contains(i.CategoryId) ? 3 :
                    complementCategories.Contains(i.CategoryId) ? 2 : 1
                )
                .ThenBy(i => i.Price)
                .ToList();

        }

        public async Task<List<Item>> GetSuggestionsV2()
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                return new List<Item>();

            // 1. Get cart items
            var cartItems = await _context.UserCarts
                .Where(c => c.UserId == userId)
                .Select(c => c.ItemId)
                .ToListAsync();

            if (!cartItems.Any())
                return new List<Item>();

            // 2. Get relationships FROM cart items
            var relationships = await _context.ItemRelationships
                .Where(r => cartItems.Contains(r.ItemId))
                .ToListAsync();

            if (!relationships.Any())
            {
                return await _context.Items
                    .Where(i =>
                        !cartItems.Contains(i.Id)
                        && i.Price < 100  
                    )
                    .OrderBy(i => i.Price)
                    .Take(10)
                    .ToListAsync();
            }

            // 3. Get related item ids
            var suggestedIds = relationships
                .Select(r => r.RelatedItemId)
                .Distinct()
                .ToList();

            // 4. Get items
            var suggestions = await _context.Items
                .Where(i => suggestedIds.Contains(i.Id) && !cartItems.Contains(i.Id))
                .ToListAsync();

            // 5. Sort by relationship type (IMPORTANT FIX)
            return suggestions
                .OrderByDescending(i =>
                    relationships.Any(r => r.RelatedItemId == i.Id && r.Type == RelationType.Additionl) ? 3 :
                    relationships.Any(r => r.RelatedItemId == i.Id && r.Type == RelationType.Complement) ? 2 : 1
                )
                .ThenBy(i => i.Price)
                .ToList();
        }
    }
}
