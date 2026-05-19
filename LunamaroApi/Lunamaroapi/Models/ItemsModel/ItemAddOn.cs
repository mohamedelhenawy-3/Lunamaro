namespace Lunamaroapi.Models.ItemsModel
{
    public class ItemAddOn
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public Item Item { get; set; } = null!;
    }
}
