namespace Lunamaroapi.DTOs
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
    }
}
