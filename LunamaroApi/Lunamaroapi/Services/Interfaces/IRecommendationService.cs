using Lunamaroapi.Models;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IRecommendationService
    {
        Task<List<Item>> GetSuggestions();

    }
}
