using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.ReservationDTO
{
    public class ReservationDto
    {        public int TableId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Guests { get; set; }
        public string? Notes { get; set; }
    }
}
