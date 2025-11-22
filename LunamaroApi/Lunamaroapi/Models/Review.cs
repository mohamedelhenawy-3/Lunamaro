namespace Lunamaroapi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
