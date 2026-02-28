using Lunamaroapi.Data;
using Lunamaroapi.DTOs.AddOnRewardDto;
using Lunamaroapi.DTOs.DiscountTierDto;
using Lunamaroapi.DTOs.WeaklyDto;
using Lunamaroapi.Helper;
using Lunamaroapi.Models.Offers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin/offers")]
public class AdminOffersController : ControllerBase
{
    private readonly AppDBContext _db;

    public AdminOffersController(AppDBContext db)
    {
        _db = db;
    }

    // ================= WEEKLY DEALS =================

    [HttpGet("weekly-deals")]
    public async Task<IActionResult> GetWeeklyDeals()
    {
        var deals = await _db.WeeklyDeals
            .Include(w => w.Product).Where(x=>x.IsActive)
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Weekly deals retrieved successfully",
            Data = deals
        });
    }

    [HttpPost("weekly-deals")]
    public async Task<IActionResult> CreateWeeklyDeal([FromBody] CreateWeeklyDealDTO dto)
    {
        var deal = new WeeklyDeal
        {
            ProductId = dto.ProductId,
            DiscountPercentage = dto.DiscountPercentage,
            ExpiryDate = dto.ExpiryDate,
            IsActive = dto.IsActive
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
    [HttpPatch("weekly-deals/{id}/activate")]
    public async Task<IActionResult> ActivateWeeklyDeal(int id)
    {
        var deal = await _db.WeeklyDeals.FindAsync(id);

        if (deal == null)
            return NotFound();

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

    // ================= DISCOUNT TIERS =================

    [HttpGet("discount-tiers")]
    public async Task<IActionResult> GetDiscountTiers()
    {
        var tiers = await _db.DiscountTiers.ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Discount tiers retrieved successfully",
            Data = tiers
        });
    }

    // ================= ADD ON REWARDS =================

    [HttpGet("add-on-rewards")]
    public async Task<IActionResult> GetAddOnRewards()
    {
        var rewards = await _db.AddOnRewards
            .Include(r => r.FreeProduct) // if you want product details
            .ToListAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Add-on rewards retrieved successfully",
            Data = rewards
        });
    }
    [HttpPut("weekly-deals/{id}")]
    public async Task<IActionResult> UpdateWeeklyDeal(int id, [FromBody] UpdateWeeklyDealDTO dto)
    {
        var existing = await _db.WeeklyDeals.FindAsync(id);
        if (existing == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Weekly deal not found"
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

    // ================= DISCOUNT TIERS =================

    [HttpPost("discount-tiers")]
    public async Task<IActionResult> CreateDiscountTier([FromBody] CreateDiscountTierDTO dto)
    {
        var tier = new DiscountTier
        {
            MinimumAmount = dto.MinimumAmount,
            DiscountAmount = dto.DiscountAmount,
            IsActive = dto.IsActive
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

    [HttpPut("discount-tiers/{id}")]
    public async Task<IActionResult> UpdateDiscountTier(int id, [FromBody] UpdateDiscountTierDTO dto)
    {
        var existing = await _db.DiscountTiers.FindAsync(id);
        if (existing == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Discount tier not found"
            });
        }

        existing.MinimumAmount = dto.MinimumAmount;
        existing.DiscountAmount = dto.DiscountAmount;
        existing.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<DiscountTier>
        {
            Success = true,
            Message = "Discount tier updated successfully",
            Data = existing
        });
    }

    [HttpDelete("discount-tiers/{id}")]
    public async Task<IActionResult> DeleteDiscountTier(int id)
    {
        var tier = await _db.DiscountTiers.FindAsync(id);
        if (tier == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Discount tier not found"
            });
        }

        _db.DiscountTiers.Remove(tier);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Discount tier deleted successfully"
        });
    }
    [HttpPatch("discount-tiers/{id}/activate")]
    public async Task<IActionResult> ActivateDiscountTier(int id)
    {
        var tier = await _db.DiscountTiers.FindAsync(id);

        if (tier == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Discount tier not found"
            });
        }

        tier.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Discount tier activated successfully"
        });
    }
    [HttpPatch("discount-tiers/{id}/deactivate")]
    public async Task<IActionResult> DeactivateDiscountTier(int id)
    {
        var tier = await _db.DiscountTiers.FindAsync(id);

        if (tier == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Discount tier not found"
            });
        }

        tier.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Discount tier deactivated successfully"
        });
    }
    // ================= ADD ON REWARDS =================

    [HttpPost("add-on-rewards")]
    public async Task<IActionResult> CreateAddOnReward([FromBody] CreateAddOnRewardDTO dto)
    {
        var reward = new AddOnReward
        {
            MinimumAmount = dto.MinimumAmount,
            FreeProductId = dto.FreeProductId,
            IsActive = dto.IsActive
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

    [HttpPut("add-on-rewards/{id}")]
    public async Task<IActionResult> UpdateAddOnReward(int id, [FromBody] UpdateAddOnRewardDTO dto)
    {
        var existing = await _db.AddOnRewards.FindAsync(id);
        if (existing == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Add-on reward not found"
            });
        }

        existing.MinimumAmount = dto.MinimumAmount;
        existing.FreeProductId = dto.FreeProductId;
        existing.IsActive = dto.IsActive;

        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<AddOnReward>
        {
            Success = true,
            Message = "Add-on reward updated successfully",
            Data = existing
        });
    }

    [HttpDelete("add-on-rewards/{id}")]
    public async Task<IActionResult> DeleteAddOnReward(int id)
    {
        var reward = await _db.AddOnRewards.FindAsync(id);
        if (reward == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Add-on reward not found"
            });
        }

        _db.AddOnRewards.Remove(reward);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Add-on reward deleted successfully"
        });
    }
    [HttpPatch("add-on-rewards/{id}/activate")]
    public async Task<IActionResult> ActivateAddOnReward(int id)
    {
        var reward = await _db.AddOnRewards.FindAsync(id);

        if (reward == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Add-on reward not found"
            });
        }

        reward.IsActive = true;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Add-on reward activated successfully"
        });
    }
    [HttpPatch("add-on-rewards/{id}/deactivate")]
    public async Task<IActionResult> DeactivateAddOnReward(int id)
    {
        var reward = await _db.AddOnRewards.FindAsync(id);

        if (reward == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Add-on reward not found"
            });
        }

        reward.IsActive = false;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Add-on reward deactivated successfully"
        });
    }
}