namespace Lunamaroapi.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }

        public int TableId { get; set; }
        public Table Table { get; set; }

        public DateTime ReservationStart { get; set; }
        public DateTime ReservationEnd { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
