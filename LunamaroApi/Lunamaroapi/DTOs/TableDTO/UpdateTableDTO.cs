using Lunamaroapi.Models;

namespace Lunamaroapi.DTOs.TableDTO
{
    public class UpdateTableDTO
    {
        public string TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Location { get; set; }

    }
}
