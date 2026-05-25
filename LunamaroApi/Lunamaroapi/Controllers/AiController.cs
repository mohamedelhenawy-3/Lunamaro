using Lunamaroapi.Models.Ai;
using Lunamaroapi.Repositories.Interfaces;
using Lunamaroapi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lunamaroapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AiController : ControllerBase
    {
        private readonly IAiChatService _aiChatService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IItemRepository _itemRepository;
        public AiController(IAiChatService aiChatService, IHttpClientFactory httpClientFactory, IItemRepository itemRepository
,
          IConfiguration configuration)
        {
            _itemRepository = itemRepository;
            _aiChatService = aiChatService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                var groqSettings = _configuration.GetSection("Groq");
                var apiKey = groqSettings["ApiKey"];
                var apiUrl = groqSettings["ApiUrl"];
                var model = groqSettings["Model"];

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = model,
                    messages = new[] {
                new { role = "user", content = "say hello" }
            },
                    max_tokens = 50
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var body = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    statusCode = (int)response.StatusCode,
                    body = body,
                    apiKeyExists = !string.IsNullOrEmpty(apiKey),
                    apiKeyStart = apiKey?.Substring(0, Math.Min(10, apiKey?.Length ?? 0))
                });
            }
            catch (Exception ex)
            {
                return Ok(new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] AiChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Message is required");

            try
            {
                var response = await _aiChatService.GetResponseAsync(request);
                return Ok(new
                {
                    message = response.Message,
                    pendingReservation = response.PendingReservation,
                    reservationCompleted = response.ReservationCompleted,
                    requiresLogin = response.RequiresLogin
                });
            }
            catch (Exception ex)
            {
                // ✅ return real error temporarily
                return StatusCode(500, new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
        [HttpPost("chat-debug")]
        public async Task<IActionResult> ChatDebug([FromBody] AiChatRequest request)
        {
            try
            {
                // Step 1 - test DB
                var items = await _itemRepository.GetAllItemsAsync();
                var itemCount = items.Count();

                // Step 2 - test Groq with menu
                var menuSummary = items.Take(3).Select(i => new {
                    name = i.Name,
                    price = i.Price,
                    description = i.Description,
                    category = i.Category?.Name ?? ""
                });

                var groqSettings = _configuration.GetSection("Groq");
                var apiKey = groqSettings["ApiKey"];
                var model = groqSettings["Model"];
                var apiUrl = groqSettings["ApiUrl"];

                var messages = new List<object>
        {
            new { role = "system", content = $"You are a restaurant assistant. Menu: {JsonSerializer.Serialize(menuSummary)}" },
            new { role = "user", content = request.Message }
        };

                var requestBody = new
                {
                    model = model,
                    messages = messages,
                    max_tokens = 200
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", apiKey);

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);
                var body = await response.Content.ReadAsStringAsync();

                return Ok(new
                {
                    itemCount,
                    groqStatus = (int)response.StatusCode,
                    groqBody = body
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    error = ex.Message,
                    inner = ex.InnerException?.Message,
                    stack = ex.StackTrace
                });
            }
        }
    }
}
