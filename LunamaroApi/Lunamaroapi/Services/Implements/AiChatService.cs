using Lunamaroapi.Data;
using Lunamaroapi.Models.Ai;
using Lunamaroapi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lunamaroapi.Services.Implements
{
    public class AiChatService : IAiChatService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IItemService _itemService;
        private readonly IReservation _reservationService;
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AiChatService> _logger;

        public AiChatService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IItemService itemService,
            IReservation reservationService,
            AppDBContext db,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AiChatService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _itemService = itemService;
            _reservationService = reservationService;
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<AiChatResponse> GetResponseAsync(AiChatRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var isLoggedIn = !string.IsNullOrEmpty(userId);

                // ✅ Check if reservation intent but not logged in
                if (IsReservationIntent(request.Message) && !isLoggedIn)
                {
                    return new AiChatResponse
                    {
                        Message = "To make a reservation, you need to be logged in first. Please login and come back to complete your booking.",
                        RequiresLogin = true
                    };
                }

                // ✅ Handle confirmation of pending reservation
                if (request.PendingReservation?.AwaitingConfirmation == true && IsConfirmation(request.Message))
                {
                    return await CompleteReservation(request.PendingReservation, userId!);
                }

                // ✅ Handle cancellation of pending reservation
                if (request.PendingReservation?.AwaitingConfirmation == true && IsCancellation(request.Message))
                {
                    return new AiChatResponse
                    {
                        Message = "No problem! Reservation cancelled. Is there anything else I can help you with?",
                        PendingReservation = null
                    };
                }

                // ✅ Load all data
                var allItems = await _itemService.GetAllItemsAsync();
                var specialItems = await _itemService.GetSpecialItems();
                var popularItems = await _itemService.ExplorePopularItems();

                // ✅ Load offers
                var now = DateTime.UtcNow;
                var weeklyDeals = await _db.WeeklyDeals
                    .Include(w => w.Product)
                    .Where(x => x.IsActive && x.ExpiryDate > now)
                    .Select(d => new {
                        productName = d.Product.Name,
                        originalPrice = d.Product.Price,
                        discountPercent = d.DiscountPercentage,
                        finalPrice = d.Product.Price - (d.Product.Price * d.DiscountPercentage / 100),
                        expiresAt = d.ExpiryDate
                    }).ToListAsync();

                var discountTiers = await _db.DiscountTiers
                    .Where(t => t.IsActive)
                    .Select(t => new {
                        minimumOrder = t.MinimumAmount,
                        discountAmount = t.DiscountAmount
                    }).ToListAsync();

                var addOnRewards = await _db.AddOnRewards
                    .Include(r => r.FreeProduct)
                    .Where(r => r.IsActive)
                    .Select(r => new {
                        minimumOrder = r.MinimumAmount,
                        freeItem = r.FreeProduct.Name
                    }).ToListAsync();

                // ✅ Check if reservation related — load available tables
                string availableTablesSection = "";
                if (IsReservationIntent(request.Message))
                {
                    var parsed = TryParseDateTime(request.Message);
                    if (parsed.HasValue)
                    {
                        var tables = await _reservationService.GetAvailableTablesAsync(
                            parsed.Value.start,
                            parsed.Value.end,
                            parsed.Value.guests
                        );
                        if (tables.Any())
                            availableTablesSection = $"\nAvailable Tables for requested time:\n{JsonSerializer.Serialize(tables)}";
                        else
                            availableTablesSection = "\nNo tables available for that time. Suggest another time.";
                    }
                }

                // ✅ Build system prompt
                var systemPrompt = $@"
You are a smart and friendly AI assistant for Lunamaro Restaurant.
Today's date: {DateTime.Now:dddd, MMMM dd yyyy}
Current time: {DateTime.Now:hh:mm tt}
User is logged in: {isLoggedIn}

=== RESTAURANT INFO ===
Name: Lunamaro Restaurant
Location: Ahmed Oraby Street, Giza, Egypt
Phone: +20 015 5660 59
Working Hours: 9:00 AM to 12:00 AM daily
Parking: Available
Seating: Indoor and outdoor available
Payment: Cash and credit cards accepted
Kids menu: Available

=== RESERVATION RULES ===
- Must be logged in to reserve
- Working hours: 9:00 AM to 12:00 AM
- Must book at least 1 hour in advance
- Max duration: 2 hours
- Time slots: every 30 minutes (9:00, 9:30, 10:00...)
- To complete booking need: TableId, StartTime, EndTime, Guests

=== RESERVATION FLOW ===
1. Ask for date, time, number of guests if not provided
2. Show available tables from the Available Tables section
3. Let user pick a table
4. Summarize and ask for confirmation
5. When ready to confirm, add this EXACTLY at the end:
[RESERVATION_DATA: tableId=X, tableNumber=X, startTime=YYYY-MM-DDTHH:mm:ss, endTime=YYYY-MM-DDTHH:mm:ss, guests=N]

=== FULL MENU ({allItems.Count()} items) ===
{JsonSerializer.Serialize(allItems.Select(i => new { i.Name, i.Price, i.Description, category = i.CategoryId }))}

=== CHEF'S SPECIAL ITEMS ===
{JsonSerializer.Serialize(specialItems.Select(i => new { i.Name, i.Price, i.Description }))}

=== BEST SELLERS / POPULAR ITEMS ===
{JsonSerializer.Serialize(popularItems.Select(i => new { i.Name, i.Price, i.Description }))}

=== CURRENT OFFERS ===
Weekly Deals (limited time discounts):
{JsonSerializer.Serialize(weeklyDeals)}

Discount Tiers (order more, save more):
{JsonSerializer.Serialize(discountTiers)}

Add-On Rewards (free items with qualifying orders):
{JsonSerializer.Serialize(addOnRewards)}

{availableTablesSection}

=== DIETARY & ALLERGY POLICY ===
- Always recommend users inform staff about allergies on arrival
- Never guarantee allergen-free dishes
- Suggest items by description when asked about dietary needs
- For vegetarian: suggest items with no meat in description
- For spicy: check description for chili/spicy/buffalo/hot

=== CANCELLATION POLICY ===
- Cancel at least 2 hours before reservation time
- Users can cancel from My Reservations page or ask here

=== RULES ===
- Only recommend items that exist in the menu above
- Be friendly, helpful and concise
- Respond in the same language the user uses (Arabic or English)
- For non-restaurant questions, politely redirect
- Never make up items or prices
- If user asks for cheapest item, find lowest price in menu
- If user asks for most expensive, find highest price
- Always mention current offers when relevant
";

                // ✅ Build messages
                var messages = new List<object>
                {
                    new { role = "system", content = systemPrompt }
                };

                foreach (var msg in request.History
                    .Where(m => m.Role == "user" || m.Role == "assistant")
                    .TakeLast(8))
                {
                    messages.Add(new { role = msg.Role, content = msg.Content });
                }

                messages.Add(new { role = "user", content = request.Message });

                // ✅ Call Groq
                var groqSettings = _configuration.GetSection("Groq");
                var requestBody = new
                {
                    model = groqSettings["Model"],
                    messages = messages,
                    max_tokens = 700,
                    temperature = 0.7
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", groqSettings["ApiKey"]);

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(groqSettings["ApiUrl"], content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Groq error: {body}", responseString);
                    return new AiChatResponse { Message = "Sorry, I am having trouble right now. Please try again." };
                }

                var groqResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
                var aiMessage = groqResponse
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "";

                // ✅ Extract reservation data if present
                var pendingReservation = ExtractReservationData(aiMessage);
                var cleanMessage = CleanReservationTag(aiMessage);

                return new AiChatResponse
                {
                    Message = cleanMessage,
                    PendingReservation = pendingReservation,
                    ReservationCompleted = false,
                    RequiresLogin = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AiChatService error");
                return new AiChatResponse { Message = "Sorry, something went wrong. Please try again." };
            }
        }

        // ✅ Complete the reservation
        private async Task<AiChatResponse> CompleteReservation(PendingReservation pending, string userId)
        {
            try
            {
                var dto = new Lunamaroapi.DTOs.ReservationDTO.ReservationDto
                {
                    TableId = pending.TableId!.Value,
                    StartTime = pending.StartTime!.Value,
                    EndTime = pending.EndTime!.Value,
                    Guests = pending.Guests!.Value,
                    Notes = "Booked via AI Assistant"
                };

                await _reservationService.Add(dto);

                return new AiChatResponse
                {
                    Message = $"Your reservation is confirmed!\n\nTable: {pending.TableNumber}\nDate: {dto.StartTime:dddd, MMMM dd}\nTime: {dto.StartTime:hh:mm tt} - {dto.EndTime:hh:mm tt}\nGuests: {dto.Guests}\n\nA confirmation email will be sent to you. We look forward to seeing you at Lunamaro!",
                    PendingReservation = null,
                    ReservationCompleted = true
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new AiChatResponse
                {
                    Message = "You need to be logged in to make a reservation. Please login first.",
                    RequiresLogin = true
                };
            }
            catch (Exception ex)
            {
                return new AiChatResponse
                {
                    Message = $"Sorry, could not complete the reservation: {ex.Message}. Please try the reservation page directly.",
                    PendingReservation = null
                };
            }
        }

        // ✅ Try to parse date/time/guests from message
        private (DateTime start, DateTime end, int guests)? TryParseDateTime(string message)
        {
            try
            {
                var tomorrow = DateTime.Today.AddDays(1);
                var guestsMatch = Regex.Match(message, @"(\d+)\s*(guests?|people|persons?|أشخاص|شخص)");
                var guests = guestsMatch.Success ? int.Parse(guestsMatch.Groups[1].Value) : 2;

                var timeMatch = Regex.Match(message, @"(\d{1,2}):?(\d{2})?\s*(am|pm|AM|PM)?");
                DateTime start;

                if (timeMatch.Success)
                {
                    var hour = int.Parse(timeMatch.Groups[1].Value);
                    var minute = timeMatch.Groups[2].Success ? int.Parse(timeMatch.Groups[2].Value) : 0;
                    var isPm = timeMatch.Groups[3].Value.ToLower() == "pm";
                    if (isPm && hour < 12) hour += 12;
                    start = tomorrow.AddHours(hour).AddMinutes(minute);
                }
                else
                {
                    start = tomorrow.AddHours(19); // default 7 PM
                }

                return (start, start.AddHours(2), guests);
            }
            catch { return null; }
        }

        private PendingReservation? ExtractReservationData(string message)
        {
            try
            {
                var match = Regex.Match(message,
                    @"\[RESERVATION_DATA: tableId=(\d+), tableNumber=([^,]+), startTime=([^,]+), endTime=([^,]+), guests=(\d+)\]");

                if (!match.Success) return null;

                return new PendingReservation
                {
                    TableId = int.Parse(match.Groups[1].Value),
                    TableNumber = match.Groups[2].Value.Trim(),
                    StartTime = DateTime.Parse(match.Groups[3].Value),
                    EndTime = DateTime.Parse(match.Groups[4].Value),
                    Guests = int.Parse(match.Groups[5].Value),
                    AwaitingConfirmation = true
                };
            }
            catch { return null; }
        }

        private string CleanReservationTag(string message)
        {
            return Regex.Replace(message, @"\[RESERVATION_DATA:[^\]]+\]", "").Trim();
        }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private bool IsReservationIntent(string message)
        {
            var keywords = new[] { "reserv", "book", "table", "seat", "حجز", "طاولة", "احجز", "أحجز" };
            return keywords.Any(k => message.ToLower().Contains(k));
        }

        private bool IsConfirmation(string message)
        {
            var keywords = new[] { "yes", "confirm", "ok", "sure", "please", "do it", "yep", "نعم", "تمام", "اكيد", "أكيد", "موافق", "يلا", "احجز" };
            return keywords.Any(k => message.ToLower().Contains(k));
        }

        private bool IsCancellation(string message)
        {
            var keywords = new[] { "no", "cancel", "stop", "never mind", "لا", "الغ", "إلغاء", "لأ" };
            return keywords.Any(k => message.ToLower().Contains(k));
        }
    }
}