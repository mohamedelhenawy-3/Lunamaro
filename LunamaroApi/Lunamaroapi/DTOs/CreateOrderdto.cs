namespace Lunamaroapi.DTOs
{
    public class CreateOrderdto
    {
        public string Name { get; set; }

        public string PhoneNumber { get; set; }
        public string DeliveryStreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int PostalCode { get; set; }
    }
}
