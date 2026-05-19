using Lunamaroapi.Models.CategoryEnums;

namespace Lunamaroapi.Models.ItemsModel
{
    public class ItemRelationship
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int RelatedItemId { get; set; }
        public RelationType Type { get; set; } 
        public Item Item { get; set; } = null!;
        public Item RelatedItem { get; set; } = null!;
    }
}
