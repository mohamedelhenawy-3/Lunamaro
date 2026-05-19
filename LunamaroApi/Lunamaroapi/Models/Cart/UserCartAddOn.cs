
using Lunamaroapi.Models.ItemsModel;

namespace Lunamaroapi.Models.Cart
{
    public class UserCartAddOn
    {
        public int Id { get; set; }

        public int UserCartId { get; set; }
        public int AddOnId { get; set; }

        public UserCart UserCart { get; set; } = null!;
        public ItemAddOn AddOn { get; set; } = null!;
    }
}
