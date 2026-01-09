using Lunamaroapi.Data;
using Lunamaroapi.DTOs.Item;
using Lunamaroapi.DTOs.Review;
using Lunamaroapi.Models;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Lunamaroapi.Services
{
    public class ReviewsService : IReview
    {

        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewsService(AppDBContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;

        }


        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }



        public async Task CreateReview(CreateReview dto)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5.");
            }


            var review = new Review
            {
                UserId = GetCurrentUserId(),
                Rating = dto.Rating,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };


            _db.Reviews.AddAsync(review);
            await _db.SaveChangesAsync();

        }


        public async Task<ReturnedAllReviews> AllReview()
        {
            var reviews = await _db.Reviews.ToListAsync();
            var returnedReviews = reviews.Select(r => new ReturnedReview
            {
                Name = GetUserName(r.UserId),
                Rating = r.Rating,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            }).ToList();



            var averageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0;



            return new ReturnedAllReviews
            {
                AverageRating = Math.Round(averageRating, 1), 
                TotalReviews = reviews.Count,
                Reviews = returnedReviews
            };
        }



        private string GetUserName(string userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            return user?.UserName ?? "Anonymous";
        }

        public async Task UpdateAsync(UpdateReviewDTO dto, int id)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var userId = GetCurrentUserId();
            var review = await _db.Reviews.FindAsync(id);

            if (review == null)
                throw new ArgumentException("Review not found.");

            if (review.UserId != userId)
                throw new ArgumentException("You are not allowed to edit this review.");

            review.Rating = dto.Rating;
            review.Content = dto.Content;
            review.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var userId = GetCurrentUserId();
            var review = await _db.Reviews.FindAsync(id);

            if (review == null)
                throw new ArgumentException("Review not found.");

            if (review.UserId != userId)
                throw new ArgumentException("You are not allowed to edit this review.");

            _db.Remove(review);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReturnedReview>> AdminGetAllReviews()
        {
            var reviews = await _db.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reviews.Select(r => new ReturnedReview
            {
                Name = GetUserName(r.UserId),
                Rating = r.Rating,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            });
        }


        public async Task AdminDeleteReview(int id)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found.");

            _db.Reviews.Remove(review);
            await _db.SaveChangesAsync();
        }

        public async Task AdminUpdateReview(int id, UpdateReviewDTO dto)
        {
            var review = await _db.Reviews.FindAsync(id);
            if (review == null)
                throw new ArgumentException("Review not found.");

            review.Rating = dto.Rating;
            review.Content = dto.Content;
            review.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        public async Task<ReturnedAllReviews> LatestReviews()
        {
            var reviews = await _db.Reviews
                .OrderByDescending(r => r.CreatedAt)
                .Take(6)         // latest 5 reviews
                .ToListAsync();

            var returnedReviews = reviews.Select(r => new ReturnedReview
            {
                Name = GetUserName(r.UserId),
                Rating = r.Rating,
                Content = r.Content,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new ReturnedAllReviews
            {
                AverageRating = returnedReviews.Any() ? returnedReviews.Average(r => r.Rating) : 0,
                TotalReviews = returnedReviews.Count,
                Reviews = returnedReviews
            };
        }


        //////////////////////////// Admin /|||||||||||||
        ///


    }
}
