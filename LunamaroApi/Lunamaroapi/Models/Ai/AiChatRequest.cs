namespace Lunamaroapi.Models.Ai
{
    public class AiChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<ChatMessage> History { get; set; } = new();
        public PendingReservation? PendingReservation { get; set; }
        public bool IsUserLoggedIn { get; set; }
    }
}
