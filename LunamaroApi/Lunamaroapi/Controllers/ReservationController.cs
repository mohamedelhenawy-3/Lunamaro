using Lunamaroapi.Data;
using Lunamaroapi.DTOs.ReservationDTO;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservation _reservationService;
        public ReservationController(IReservation reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reservations = await _reservationService.GetAllAsync();
            return Ok(reservations);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var resarvation = await _reservationService.GetByIdAsync(id);
            return Ok(resarvation);

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationDto dto)
        {
            try
            {
                var reservation = await _reservationService.Add(dto);
                return Ok(new { message = "Reservation created successfully", reservation });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var success = await _reservationService.ApproveAsync(id);
            if(!success) return NotFound(new { message = "Reservation not found" });
            return Ok(new { message = "Reservation approved successfully" });

        }
        [HttpPut("reject/{id}")]
        public async Task<IActionResult> Reject(int id)
        {
            var success = await _reservationService.RejectAsync(id);

            if (!success)
                return NotFound(new { message = "Reservation not found" });

            return Ok(new { message = "Reservation Reject successfully" });
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            try
            {
                await _reservationService.DeleteAsync(id);
                return Ok("Reservation deleted successfully");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            await _reservationService.UpdateStatusAsync(dto, id);
            return Ok(new { message = "Status updated successfully" });
        }


    }
}
