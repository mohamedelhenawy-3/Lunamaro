using Lunamaroapi.Data;
using Lunamaroapi.DTOs;
using Lunamaroapi.Models;
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
        private readonly AppDBContext _context;

        public ReservationController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("Make")]
        public async Task<IActionResult> MakeReservation([FromBody] ReservationRequest request)
        {
            var nowUtc = DateTime.UtcNow;

            if (request.ReservationStart >= request.ReservationEnd)
            {
                return BadRequest("Start time must be before end time.");
            }

            if (request.ReservationStart.ToUniversalTime() <= nowUtc)
            {
                return BadRequest("Reservation start time must be in the future.");
            }

            bool conflict = await _context.Reservations.AnyAsync(r => r.TableId == request.TableId && r.Status == "reserved" &&
                (
                    (request.ReservationStart < r.ReservationEnd && request.ReservationStart >= r.ReservationStart) ||
                    (request.ReservationEnd > r.ReservationStart && request.ReservationEnd <= r.ReservationEnd) ||
                    (request.ReservationStart <= r.ReservationStart && request.ReservationEnd >= r.ReservationEnd)
                ));

            if (conflict)
            {
                return Conflict("This table is already reserved in the selected time.");
            }

            var reservation = new Reservation
            {
                FullName = request.FullName,
                Phone = request.Phone,
                TableId = request.TableId,
                ReservationStart = request.ReservationStart.ToUniversalTime(),
                ReservationEnd = request.ReservationEnd.ToUniversalTime(),
                Status = "reserved"
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return Ok("Reservation successful.");
        }

    }
}
