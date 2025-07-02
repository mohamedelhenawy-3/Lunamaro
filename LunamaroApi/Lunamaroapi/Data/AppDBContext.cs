using Lunamaroapi.Data.Configuration;
using Lunamaroapi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Data
{
    public class AppDBContext :IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
       public  DbSet<ApplicationUser> Users { get; set; }
      public   DbSet<Item> Items { get; set; }
      public   DbSet<Category> Categories { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserOrderHeader> UserOrderHeaders { get; set; }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // important for Identity tables

            // Apply Fluent API configurations
            modelBuilder.ApplyConfiguration(new itemConfig());
            modelBuilder.ApplyConfiguration(new categoryConfig());
            modelBuilder.ApplyConfiguration(new UserCartConfig());
            modelBuilder.Entity<OrderItem>()
           .HasOne(oi => oi.Order)
           .WithMany(o => o.OrderItems)
           .HasForeignKey(oi => oi.OrderId);


        

            var tables = new List<Table>();

            for (int i = 1; i <= 50; i++)
            {
                tables.Add(new Table
                {
                    Id = i,
                    Capacity = (i % 3 == 0) ? 6 : (i % 2 == 0) ? 4 : 2,
                    Location = (int)(i <= 15
          ? LocationType.Terrace
          : i <= 35
              ? LocationType.Indoor
              : LocationType.WindowArea)
                });

            }

            modelBuilder.Entity<Table>().HasData(tables);
        }

      
        

    }
}
