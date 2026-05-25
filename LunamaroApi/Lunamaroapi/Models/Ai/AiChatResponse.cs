namespace Lunamaroapi.Models.Ai
{
    public class AiChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public PendingReservation? PendingReservation { get; set; }
        public bool ReservationCompleted { get; set; }
        public bool RequiresLogin { get; set; }
    }
}
