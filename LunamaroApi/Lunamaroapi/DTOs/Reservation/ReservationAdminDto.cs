using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.ReservationDTO
{
    public class ReservationAdminDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public string UserEmail { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Guests { get; set; }
        public string? Notes { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
