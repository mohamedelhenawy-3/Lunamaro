using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.ReservationDTO
{
    public class UserReservationDTO
    {
        public int Id { get; set; }
        public string TableNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
    
    }
}
