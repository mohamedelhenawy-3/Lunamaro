namespace Lunamaroapi.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public int TableId { get; set; }

        // User-selected times
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int Guests { get; set; }
        public string? Notes { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Table? Table { get; set; }
    }
}
