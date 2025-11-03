using Lunamaroapi.DTOs.ReservationDTO;
using Lunamaroapi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IReservation
    {
        Task<IEnumerable<ReservationAdminDto>> GetAllAsync();
        Task<Reservation?> GetByIdAsync(int id);
        Task <ReservationDto>Add(ReservationDto resDto);
        Task UpdateAsync(ReservationDto resDto , int id);
        Task DeleteAsync(int id);
        Task<bool> IsAvailableAsync(int TableId, DateTime start, DateTime end);
        Task SaveAsync();
        Task<bool> ApproveAsync(int id); // Admin
        Task<bool> RejectAsync(int id);  // Admin
        //vIEWaVALIBLEtABLE

        Task UpdateStatusAsync(UpdateStatusDto dto, int id);


        
     
    }
}
