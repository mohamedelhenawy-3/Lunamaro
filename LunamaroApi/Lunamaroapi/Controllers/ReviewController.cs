using Lunamaroapi.DTOs.Review;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {


        private readonly IReview _reviewservice;
        public ReviewController(IReview re)
        {
            _reviewservice = re;
        }
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] CreateReview dto)
        {
            try
            {
                await _reviewservice.CreateReview(dto);
                return Ok(new { message = "Review added successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the review." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllReviews()
        {
            var result = await _reviewservice.AllReview();
            return Ok(result);
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, UpdateReviewDTO dto)
        {
            try
            {
                await _reviewservice.UpdateAsync(dto, id);
                return Ok(new { message = "Review updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                await _reviewservice.RemoveAsync(id);
                return Ok(new { message = "Review deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the review." });
            }
        }
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var data = await _reviewservice.LatestReviews();
            return Ok(data);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllAdminReviews()
        {
            var reviews = await _reviewservice.AdminGetAllReviews();
            return Ok(reviews);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/{id}")]
        public async Task<IActionResult> AdminDelete(int id)
        {
            await _reviewservice.AdminDeleteReview(id);
            return Ok(new { message = "Review deleted by admin." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id}")]
        public async Task<IActionResult> AdminUpdate(int id, UpdateReviewDTO dto)
        {
            await _reviewservice.AdminUpdateReview(id, dto);
            return Ok(new { message = "Review updated by admin." });
        }

    }
}
