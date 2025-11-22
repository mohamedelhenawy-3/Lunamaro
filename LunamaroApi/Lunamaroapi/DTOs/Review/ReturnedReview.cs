namespace Lunamaroapi.DTOs.Review
{
    public class ReturnedReview
    {
        public string Name { get; set; }
        public int Rating { get; set; } // 1 to 5
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
