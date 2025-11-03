namespace Lunamaroapi.Models
{
    public class Table
    {
        public int Id { get; set; }
        public string TableNumber { get; set; }
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public TableStatus IsAvailable = TableStatus.Available;
        public ICollection<Reservation>? Reservations { get; set; }

    }
}
