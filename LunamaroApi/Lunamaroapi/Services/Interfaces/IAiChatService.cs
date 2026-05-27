using Lunamaroapi.Models.Ai;

namespace Lunamaroapi.Services.Interfaces
{
    public interface IAiChatService
    {
        Task<AiChatResponse> GetResponseAsync(AiChatRequest request);



    }
}
