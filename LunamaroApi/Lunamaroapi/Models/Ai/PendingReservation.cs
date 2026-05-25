namespace Lunamaroapi.Models.Ai
{
    public class PendingReservation
    {
        public int? TableId { get; set; }
        public string? TableNumber { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Guests { get; set; }
        public string? Notes { get; set; }
        public bool AwaitingConfirmation { get; set; }
    }
}
