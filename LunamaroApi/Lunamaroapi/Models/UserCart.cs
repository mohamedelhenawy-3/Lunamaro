namespace Lunamaroapi.Models
{
    public class UserCart
    {
       
            public int Id { get; set; }

            public string UserId { get; set; }
            public ApplicationUser User { get; set; }

            public int ItemId { get; set; }
            public Item Item { get; set; }

            public int Quantity { get; set; }
            public DateTime AddedAt { get; set; } = DateTime.Now;
       

    }
}
