using Lunamaroapi.Data;
using Lunamaroapi.DTOs.ReservationDTO;
using Lunamaroapi.DTOs.TableDTO;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace Lunamaroapi.Services
{
    public class ReservationServices : IReservation
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReservationServices(AppDBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ReservationDto> Add(ReservationDto dto)
        {

            var yable = await _db.Tables.FindAsync(dto.TableId);


            if (yable == null) throw new Exception("Table Not Found");

            if(dto.Guests > yable.Capacity) throw new Exception($"The selected table cannot accommodate {dto.Guests} guests. Maximum capacity is {yable.Capacity}.");



            // Validate time range
            if (dto.EndTime <= dto.StartTime)
                throw new ArgumentException("End time must be after start time");



            // Check availability
            var isAvailable = await IsAvailableAsync(dto.TableId, dto.StartTime, dto.EndTime);
            if (!isAvailable)
                throw new InvalidOperationException("Table not available at this time");

            // Create model
            var reservation = new Reservation
            {
                TableId = dto.TableId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Guests = dto.Guests,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow,
                UserId = GetCurrentUserId()
            };

            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
            return new ReservationDto
            {
                TableId = reservation.TableId,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                Guests = reservation.Guests,
                Notes = reservation.Notes
            };
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

        public async Task<bool> IsAvailableAsync(int TableId, DateTime start, DateTime end)
        {
            return !await _db.Reservations.AnyAsync(r =>
                         r.TableId == TableId  &&
                         r.Status != ReservationStatus.Cancelled
                         && r.Status != ReservationStatus.Rejected
                         // Ignore rejected
                         &&((start >= r.StartTime && start < r.EndTime) || (end >  r.StartTime && end  <= r.EndTime) || (start <= r.StartTime && end >= r.EndTime) )
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
            var reservation = await _db.Reservations.FindAsync(id);

            if (reservation == null)
                throw new Exception("Reservation not found");

            reservation.Status = dto.Status;

            await _db.SaveChangesAsync(); // ✅ Use async version
        }

        public async Task<IEnumerable<UserReservationDTO>> GetReservationByUser(string UserId)
        {
            string UserID = GetCurrentUserId();
            var reservation = await _db.Reservations.Where(r => r.UserId == UserId).Include(r => r.Table).Select(r => new UserReservationDTO
            {
                Id = r.Id,
                TableNumber = r.Table.TableNumber,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                Status= r.Status
                
            }
                ).ToListAsync();
            return reservation;
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
                    ((startTime >= r.StartTime && startTime < r.EndTime) ||
                     (endTime > r.StartTime && endTime <= r.EndTime) ||
                     (startTime <= r.StartTime && endTime >= r.EndTime))
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
    }
}
