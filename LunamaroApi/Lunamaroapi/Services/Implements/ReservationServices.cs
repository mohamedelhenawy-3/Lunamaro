using Lunamaroapi.Data;
using Lunamaroapi.DTOs.ReservationDTO;
using Lunamaroapi.DTOs.TableDTO;
using Lunamaroapi.Models;
using Lunamaroapi.Queues;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Twilio.TwiML.Messaging;

namespace Lunamaroapi.Services.Implements
{
    public delegate Task ReservationPlaceHandler(string to,Reservation reservation);

    public class ReservationServices : IReservation
    {
        private readonly EmailService _emailService;


        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReservationServices(AppDBContext db, EmailService emailService, IHttpContextAccessor httpContextAccessor,EmailService _sendmail)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<ReservationDto> Add(ReservationDto dto)
        {
            var userId = GetCurrentUserId();

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not logged in.");


            var openingTime = dto.StartTime.Date.AddHours(9);
            var closingTime = dto.StartTime.Date.AddHours(23).AddMinutes(59);

            if (dto.StartTime < openingTime || dto.EndTime > closingTime)
            {
                throw new InvalidOperationException(
                    "Reservations allowed only between 09:00 AM and 12:00 AM."
                );
            }
            if (dto.StartTime.Minute % 30 != 0 || dto.EndTime.Minute % 30 != 0)
                throw new InvalidOperationException("Reservations must be on 00 or 30 minutes.");

            
            if (dto.StartTime < DateTime.UtcNow.AddHours(1))
                throw new InvalidOperationException("Must book at least 1 hour in advance.");

            var duration = (dto.EndTime - dto.StartTime).TotalMinutes;

            if (duration <= 0)
                throw new ArgumentException("Invalid time range.");

            if (duration > 120)
                throw new ArgumentException("Max reservation is 2 hours.");

       
            var table = await _db.Tables.FindAsync(dto.TableId);

            if (table == null)
                throw new KeyNotFoundException("Table not found.");

            if (dto.Guests > table.Capacity)
                throw new ArgumentException("Too many guests for this table.");

       
            var isAvailable = await IsAvailableAsync(dto.TableId, dto.StartTime, dto.EndTime);

            if (!isAvailable)
                throw new InvalidOperationException("Table already booked.");

      
            var reservation = new Reservation
            {
                TableId = dto.TableId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Guests = dto.Guests,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Status = ReservationStatus.Pending
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();

            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var res = await _db.Reservations.FindAsync(id);
            if (res == null) return;
            _db.Reservations.Remove(res);
            await _db.SaveChangesAsync();

        }

        public async Task<IEnumerable<ReservationAdminDto>> GetAllAsync()
        {
            var ress = await _db.Reservations.Include(c => c.Table).Include(u => u.User).Select(u => new ReservationAdminDto
            {
               Id=u.Id,
               TableId=u.TableId,
               TableName=u.Table.TableNumber,
               UserEmail = u.User.Email,
                StartTime = u.StartTime,
                EndTime = u.EndTime,
                Guests = u.Guests,
                Notes = u.Notes,
               Status=u.Status,
                CreatedAt = u.CreatedAt
            }).ToListAsync();
            if (ress == null) throw new Exception(" No Reservations");
            return ress;
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var res = await _db.Reservations.FindAsync(id);
            if (res == null) return false;
            //res.Status = "Approved";
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(int id)
        {
            var res = await _db.Reservations.FindAsync(id);
            if (res == null) return false;
            res.Status = ReservationStatus.Rejected;
            await _db.SaveChangesAsync();
            return true;
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _db.Reservations.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<bool> IsAvailableAsync(int tableId, DateTime start, DateTime end)
        {
            var startWithBuffer = start.AddMinutes(-15);
            var endWithBuffer = end.AddMinutes(15);

            return !await _db.Reservations.AnyAsync(r =>
                r.TableId == tableId &&
                r.Status != ReservationStatus.Cancelled &&
                r.Status != ReservationStatus.Rejected &&
                start < r.EndTime.AddMinutes(15) &&
                end > r.StartTime.AddMinutes(-15)
            );
        }
        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(ReservationDto resDto, int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateStatusAsync(UpdateStatusDto dto, int id)
        {
            var reservation = await _db.Reservations
                .Include(r => r.User)
                .Include(r => r.Table)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                throw new Exception("Reservation not found");

            var previousStatus = reservation.Status;
            reservation.Status = dto.Status;
            await _db.SaveChangesAsync();

            if (reservation.User != null && !string.IsNullOrEmpty(reservation.User.Email))
            {
                if (dto.Status == ReservationStatus.Approved && previousStatus != ReservationStatus.Approved)
                {
                    EmailQueue.Queue.Enqueue((
                        reservation.User.Email,
                        "Your Lunamaro Reservation is Approved!",
                        BuildReservationConfirmationEmail(
                            reservation.User.FullName ?? reservation.User.Email,
                            reservation,
                            reservation.Table?.TableNumber ?? reservation.TableId.ToString()
                        )
                    ));
                }
                else if (dto.Status == ReservationStatus.Rejected && previousStatus != ReservationStatus.Rejected)
                {
                    EmailQueue.Queue.Enqueue((
                        reservation.User.Email,
                        "Your Lunamaro Reservation Could Not Be Confirmed",
                        BuildReservationRejectedEmail(
                            reservation.User.FullName ?? reservation.User.Email,
                            reservation
                        )
                    ));
                }
            }
        }

        public async Task<IEnumerable<UserReservationDTO>> GetReservationByUser(string userId)
        {
            return await _db.Reservations
                .Where(r => r.UserId == userId)
                .Include(r => r.Table)
                .Select(r => new UserReservationDTO
                {
                    Id = r.Id,
                    TableNumber = r.Table.TableNumber,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Status = r.Status
                })
                .ToListAsync();
        }
        public async Task<bool> CancelReservation(int ReservationId, string userid)
        {
             var reservation = await _db.Reservations
        .FirstOrDefaultAsync(r => r.Id == ReservationId && r.UserId == userid);

            if (reservation == null)
                return false; // ❌ Reservation not found or doesn’t belong to the user

            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();

            return true; // ✅ Reservation removed
        }

        public async Task<List<AvTablesDTO>> GetAvailableTablesAsync(DateTime startTime, DateTime endTime, int guests)
        {
            var candidateTables = _db.Tables
                .Where(t => t.Capacity >= guests);

            var availableTables = await candidateTables
                .Where(t => !_db.Reservations.Any(r =>
                    r.TableId == t.Id &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.Status != ReservationStatus.Rejected &&
                    (startTime >= r.StartTime && startTime < r.EndTime ||
                     endTime > r.StartTime && endTime <= r.EndTime ||
                     startTime <= r.StartTime && endTime >= r.EndTime)
                ))
                .ToListAsync();

            // Map to DTO
            return availableTables.Select(t => new AvTablesDTO
            {
                Id = t.Id,
                TableNumber = t.TableNumber,
                Capacity = t.Capacity,
                Location = t.Location
            }).ToList();
        }
        public List<string> GetAvailableTimeSlots()
        {
            var slots = new List<string>();

            var start = DateTime.Today.AddHours(9);
            var end = DateTime.Today.AddHours(12);

            while (start <= end)
            {
                slots.Add(start.ToString("hh:mm tt"));
                start = start.AddMinutes(30);
            }

            return slots;
        }




        private string BuildReservationConfirmationEmail(
          string userName,
          Reservation reservation,
          string tableNumber)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ margin:0; padding:0; background:#0f1c2e; font-family:'Segoe UI',Arial,sans-serif; }}
    .wrapper {{ max-width:600px; margin:40px auto; background:#1a2e46; border-radius:16px; overflow:hidden; border:1px solid rgba(239,176,54,0.2); }}
    .header {{ background:#1a2e46; padding:40px 40px 20px; text-align:center; border-bottom:1px solid rgba(239,176,54,0.2); }}
    .logo {{ font-size:2rem; font-weight:800; color:#EFB036; letter-spacing:-1px; }}
    .badge {{ display:inline-block; background:rgba(52,211,153,0.15); color:#34d399; border:1px solid rgba(52,211,153,0.3); padding:6px 18px; border-radius:20px; font-size:0.85rem; font-weight:600; margin-top:12px; }}
    .body {{ padding:35px 40px; }}
    .greeting {{ color:#ffffff; font-size:1.1rem; margin-bottom:20px; }}
    .detail-card {{ background:rgba(255,255,255,0.04); border:1px solid rgba(239,176,54,0.15); border-radius:12px; padding:24px; margin:20px 0; }}
    .detail-row {{ display:flex; justify-content:space-between; padding:10px 0; border-bottom:1px solid rgba(255,255,255,0.06); }}
    .detail-row:last-child {{ border-bottom:none; }}
    .detail-label {{ color:rgba(255,255,255,0.5); font-size:0.85rem; text-transform:uppercase; letter-spacing:1px; }}
    .detail-value {{ color:#EFB036; font-weight:600; font-size:0.95rem; }}
    .note {{ color:rgba(255,255,255,0.6); font-size:0.88rem; line-height:1.6; margin-top:20px; }}
    .footer {{ background:rgba(0,0,0,0.2); padding:20px 40px; text-align:center; color:rgba(255,255,255,0.3); font-size:0.8rem; }}
    .gold {{ color:#EFB036; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <div class='logo'>Lunamaro</div>
      <div class='badge'>✓ Reservation Confirmed</div>
    </div>

    <div class='body'>
      <p class='greeting'>Dear <strong class='gold'>{userName}</strong>,</p>
      <p style='color:rgba(255,255,255,0.7);line-height:1.7;'>
        Your table has been successfully reserved. We look forward to welcoming you!
      </p>

      <div class='detail-card'>
        <div class='detail-row'>
          <span class='detail-label'>Table</span>
          <span class='detail-value'>Table {tableNumber}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Date</span>
          <span class='detail-value'>{reservation.StartTime:dddd, MMMM dd yyyy}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Time</span>
          <span class='detail-value'>{reservation.StartTime:hh:mm tt} — {reservation.EndTime:hh:mm tt}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Guests</span>
          <span class='detail-value'>{reservation.Guests} {(reservation.Guests == 1 ? "Guest" : "Guests")}</span>
        </div>
        {(string.IsNullOrEmpty(reservation.Notes) ? "" : $@"
        <div class='detail-row'>
          <span class='detail-label'>Notes</span>
          <span class='detail-value'>{reservation.Notes}</span>
        </div>")}
      </div>

      <p class='note'>
        📍 <strong style='color:#fff;'>Ahmed Oraby Street, Giza, Egypt</strong><br>
        📞 +20 015 5660 59<br><br>
        If you need to cancel or modify your reservation, please contact us at least
        <strong style='color:#EFB036;'>2 hours before</strong> your scheduled time.
      </p>
    </div>

    <div class='footer'>
      © {DateTime.Now.Year} Lunamaro Restaurant. All rights reserved.<br>
      This is an automated confirmation email.
    </div>
  </div>
</body>
</html>";
        }

        private string BuildReservationRejectedEmail(string userName, Reservation reservation)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
  <meta charset='UTF-8'>
  <style>
    body {{ margin:0; padding:0; background:#0f1c2e; font-family:'Segoe UI',Arial,sans-serif; }}
    .wrapper {{ max-width:600px; margin:40px auto; background:#1a2e46; border-radius:16px; overflow:hidden; border:1px solid rgba(255,77,77,0.2); }}
    .header {{ background:#1a2e46; padding:40px 40px 20px; text-align:center; border-bottom:1px solid rgba(255,77,77,0.2); }}
    .logo {{ font-size:2rem; font-weight:800; color:#EFB036; letter-spacing:-1px; }}
    .badge {{ display:inline-block; background:rgba(255,77,77,0.15); color:#ff4d4d; border:1px solid rgba(255,77,77,0.3); padding:6px 18px; border-radius:20px; font-size:0.85rem; font-weight:600; margin-top:12px; }}
    .body {{ padding:35px 40px; }}
    .detail-card {{ background:rgba(255,255,255,0.04); border:1px solid rgba(255,77,77,0.15); border-radius:12px; padding:24px; margin:20px 0; }}
    .detail-row {{ display:flex; justify-content:space-between; padding:10px 0; border-bottom:1px solid rgba(255,255,255,0.06); }}
    .detail-row:last-child {{ border-bottom:none; }}
    .detail-label {{ color:rgba(255,255,255,0.5); font-size:0.85rem; text-transform:uppercase; letter-spacing:1px; }}
    .detail-value {{ color:#ffffff; font-weight:600; font-size:0.95rem; }}
    .footer {{ background:rgba(0,0,0,0.2); padding:20px 40px; text-align:center; color:rgba(255,255,255,0.3); font-size:0.8rem; }}
  </style>
</head>
<body>
  <div class='wrapper'>
    <div class='header'>
      <div class='logo'>Lunamaro</div>
      <div class='badge'>Reservation Not Available</div>
    </div>

    <div class='body'>
      <p style='color:#ffffff;font-size:1.1rem;'>Dear <strong style='color:#EFB036;'>{userName}</strong>,</p>
      <p style='color:rgba(255,255,255,0.7);line-height:1.7;'>
        Unfortunately we were unable to confirm your reservation for the requested time.
        The table may no longer be available.
      </p>

      <div class='detail-card'>
        <div class='detail-row'>
          <span class='detail-label'>Requested Date</span>
          <span class='detail-value'>{reservation.StartTime:dddd, MMMM dd yyyy}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Requested Time</span>
          <span class='detail-value'>{reservation.StartTime:hh:mm tt} — {reservation.EndTime:hh:mm tt}</span>
        </div>
        <div class='detail-row'>
          <span class='detail-label'>Guests</span>
          <span class='detail-value'>{reservation.Guests}</span>
        </div>
      </div>

      <p style='color:rgba(255,255,255,0.7);line-height:1.7;'>
        Please try booking a different time slot or contact us directly:<br>
        📞 <strong style='color:#EFB036;'>+20 015 5660 59</strong>
      </p>
    </div>

    <div class='footer'>
      © {DateTime.Now.Year} Lunamaro Restaurant. All rights reserved.
    </div>
  </div>
</body>
</html>";
        }


    }
}
