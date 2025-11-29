namespace Lunamaroapi.DTOs.Order
{
    public class OrderitemshistoryDTO
    {
        public string ProductName { get; set; }
        public string ImgUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal RowTotal => Quantity * Price;
    }
}
