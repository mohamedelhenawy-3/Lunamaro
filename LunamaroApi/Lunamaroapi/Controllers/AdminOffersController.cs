using Lunamaroapi.Data;
using Lunamaroapi.DTOs.AddOnRewardDto;
using Lunamaroapi.DTOs.DiscountTierDto;
using Lunamaroapi.DTOs.WeaklyDto;
using Lunamaroapi.Helper;
using Lunamaroapi.Models.Offers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/admin/offers")]
public class AdminOffersController : ControllerBase
{
    private readonly AppDBContext _db;

    public AdminOffersController(AppDBContext db)
    {
        _db = db;
    }


    [HttpGet("weekly-deals")]
    public async Task<IActionResult> GetWeeklyDeals()
    {
        var now = DateTime.UtcNow;

        var expiredDeals = await _db.WeeklyDeals
            .Where(d => d.IsActive && d.ExpiryDate <= now)
            .ToListAsync();

        foreach (var deal in expiredDeals)
            deal.IsActive = false;

        if (expiredDeals.Any())
            await _db.SaveChangesAsync();

        var deals = await _db.WeeklyDeals
            .Include(w => w.Product)
            .Where(x => x.IsActive && x.ExpiryDate > now)
            .ToListAsync();

        var result = deals.Select(d => new
        {
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
        });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Weekly deals retrieved successfully",
            Data = result
        });
    }



    [HttpGet("products/search")]
    public async Task<IActionResult> Search([FromQuery] string term )
    {
        if (string.IsNullOrWhiteSpace(term))
            return Ok(new List<object>());


        var products = await _db.Items
          .Where(p => p.Name.ToLower().Contains(term.ToLower()))
           .Select(p => new
           {
               p.Id,
               p.Name,
               p.ImageUrl,
               p.Price
           })
        .Take(10) 
        .ToListAsync();

        return Ok(products);

    }
    [HttpPost("weekly-deals")]
    public async Task<IActionResult> CreateWeeklyDeal([FromBody] CreateWeeklyDealDTO dto)
    {
        if (dto.ExpiryDate <= DateTime.UtcNow)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Expiry date must be in the future"
            });
        }

        var deal = new WeeklyDeal
        {
            ProductId = dto.ProductId,
            DiscountPercentage = dto.DiscountPercentage,
            ExpiryDate = dto.ExpiryDate,
            IsActive = true 
        };

        _db.WeeklyDeals.Add(deal);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<WeeklyDeal>
        {
            Success = true,
            Message = "Weekly deal created successfully",
            Data = deal
        });
    }

    [HttpPut("weekly-deals/{id}")]
    public async Task<IActionResult> UpdateWeeklyDeal(int id, [FromBody] UpdateWeeklyDealDTO dto)
    {
        if (dto == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid request data"
            });
        }

        var existing = await _db.WeeklyDeals.FindAsync(id);

        if (existing == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Weekly deal not found"
            });
        }

        if (dto.ExpiryDate <= DateTime.UtcNow)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Expiry date must be in the future"
            });
        }

        var duplicate = await _db.WeeklyDeals
            .AnyAsync(x => x.ProductId == dto.ProductId && x.Id != id);

        if (duplicate)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "This product already has a weekly deal"
            });
        }

        existing.ProductId = dto.ProductId;
        existing.DiscountPercentage = dto.DiscountPercentage;
        existing.ExpiryDate = dto.ExpiryDate;
        existing.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<WeeklyDeal>
        {
            Success = true,
            Message = "Weekly deal updated successfully",
            Data = existing
        });
    }
    [HttpPatch("weekly-deals/{id}/activate")]
    public async Task<IActionResult> ActivateWeeklyDeal(int id)
    {
        var deal = await _db.WeeklyDeals.FindAsync(id);

        if (deal == null)
            return NotFound();

        if (deal.ExpiryDate <= DateTime.UtcNow)
        {
            return BadRequest(new
            {
                message = "Cannot activate expired deal"
            });
        }

        deal.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Activated successfully" });
    }

    [HttpPatch("weekly-deals/{id}/deactivate")]
    public async Task<IActionResult> DeactivateWeeklyDeal(int id)
    {
        var deal = await _db.WeeklyDeals.FindAsync(id);

        if (deal == null)
            return NotFound();

        deal.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Deactivated successfully" });
    }

    [HttpDelete("weekly-deals/{id}")]
    public async Task<IActionResult> DeleteWeeklyDeal(int id)
    {
        var deal = await _db.WeeklyDeals.FindAsync(id);

        if (deal == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Weekly deal not found"
            });
        }

        _db.WeeklyDeals.Remove(deal);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Weekly deal deleted successfully"
        });
    }


    [HttpGet("discount-tiers")]
    public async Task<IActionResult> GetDiscountTiers()
    {
        var tiers = await _db.DiscountTiers
            .Where(t => t.IsActive)
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Discount tiers retrieved successfully",
            Data = tiers
        });
    }

    [HttpPost("discount-tiers")]
    public async Task<IActionResult> CreateDiscountTier(CreateDiscountTierDTO dto)
    {
        var tier = new DiscountTier
        {
            MinimumAmount = dto.MinimumAmount,
            DiscountAmount = dto.DiscountAmount,
            IsActive = true
        };

        _db.DiscountTiers.Add(tier);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<DiscountTier>
        {
            Success = true,
            Message = "Discount tier created successfully",
            Data = tier
        });
    }

    [HttpPatch("discount-tiers/{id}/activate")]
    public async Task<IActionResult> ActivateDiscountTier(int id)
    {
        var tier = await _db.DiscountTiers.FindAsync(id);

        if (tier == null)
            return NotFound();

        tier.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Activated successfully" });
    }

    [HttpPatch("discount-tiers/{id}/deactivate")]
    public async Task<IActionResult> DeactivateDiscountTier(int id)
    {
        var tier = await _db.DiscountTiers.FindAsync(id);

        if (tier == null)
            return NotFound();

        tier.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Deactivated successfully" });
    }

 

    [HttpGet("add-on-rewards")]
    public async Task<IActionResult> GetAddOnRewards()
    {
        var rewards = await _db.AddOnRewards
            .Include(r => r.FreeProduct)
            .Where(r => r.IsActive)
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Add-on rewards retrieved successfully",
            Data = rewards
        });
    }

    [HttpPost("add-on-rewards")]
    public async Task<IActionResult> CreateAddOnReward(CreateAddOnRewardDTO dto)
    {
        var reward = new AddOnReward
        {
            MinimumAmount = dto.MinimumAmount,
            FreeProductId = dto.FreeProductId,
            IsActive = true
        };

        _db.AddOnRewards.Add(reward);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<AddOnReward>
        {
            Success = true,
            Message = "Add-on reward created successfully",
            Data = reward
        });
    }

    [HttpPatch("add-on-rewards/{id}/activate")]
    public async Task<IActionResult> ActivateAddOnReward(int id)
    {
        var reward = await _db.AddOnRewards.FindAsync(id);

        if (reward == null)
            return NotFound();

        reward.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Activated successfully" });
    }

    [HttpPatch("add-on-rewards/{id}/deactivate")]
    public async Task<IActionResult> DeactivateAddOnReward(int id)
    {
        var reward = await _db.AddOnRewards.FindAsync(id);

        if (reward == null)
            return NotFound();

        reward.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Deactivated successfully" });
    }
}