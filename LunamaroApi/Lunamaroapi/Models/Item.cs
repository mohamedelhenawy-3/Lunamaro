using Lunamaroapi.Models.ItemsModel;

namespace Lunamaroapi.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }  // New field
        public string Name { get; set; }
        public int quantity { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsSpecial { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<ItemAddOn> AddOns { get; set; } = new List<ItemAddOn>();
        public ICollection<ItemRelationship> Relationships { get; set; } = new List<ItemRelationship>();

    }
}
