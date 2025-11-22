namespace Lunamaroapi.DTOs.Review
{
    public class ReturnedAllReviews
    {
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReturnedReview> Reviews { get; set; } = new();
    }
}
