namespace Lunamaroapi.Models
{
    public enum OrderStatus
    {
        Pending,        // User placed order / waiting
        Accepted,       // Restaurant approved & started preparing
        OutForDelivery, // Driver took the order
        Delivered,      // Customer received it
        Cancelled,
        Processing// Cancelled anytime before delivery 
            ,
    }

}
