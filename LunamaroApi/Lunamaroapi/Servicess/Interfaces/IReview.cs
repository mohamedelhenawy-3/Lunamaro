using Lunamaroapi.DTOs.ReservationDTO;
using Lunamaroapi.DTOs.Review;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IReview
    {

        Task CreateReview(CreateReview dto);
        Task<ReturnedAllReviews> AllReview();
        Task<ReturnedAllReviews>  LatestReviews();

        Task UpdateAsync(UpdateReviewDTO resDto, int id);
        Task RemoveAsync(int id);

        // ADMIN FEATURES
        Task<IEnumerable<ReturnedReview>> AdminGetAllReviews();
        Task AdminDeleteReview(int id);
        Task AdminUpdateReview(int id, UpdateReviewDTO dto);
    }
}
