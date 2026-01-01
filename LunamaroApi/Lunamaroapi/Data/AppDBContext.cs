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
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<Item> Items { get; set; }
      public   DbSet<Category> Categories { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserOrderHeader> UserOrderHeaders { get; set; }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // important for Identity tables

            // Apply Fluent API configurations
            modelBuilder.ApplyConfiguration(new itemConfig());
            modelBuilder.ApplyConfiguration(new categoryConfig());
            modelBuilder.ApplyConfiguration(new UserCartConfig());
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.UserOrderHeader)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.UserOrderHeaderId);




            modelBuilder.Entity<UserOrderHeader>()
          .HasIndex(u => u.TemporaryKey)
          .IsUnique();

            modelBuilder.Entity<Reservation>().HasOne(r => r.User)
                .WithMany(x => x.Reservations)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>().HasIndex(r => new { r.TableId, r.StartTime }).IsUnique();
            modelBuilder.Entity<Reservation>().Property(x => x.Status).HasConversion<string>();
            modelBuilder.Entity<Table>().Property(x => x.IsAvailable).HasConversion<string>();


            modelBuilder.Entity<UserOrderHeader>().Property(x => x.OrderStatus).HasConversion<string>();
 //           modelBuilder.Entity<Table>().HasData(
 //    new Table { Id = 1, TableNumber = "T1", Capacity = 2, Location = "Indoor",Status. },
 //    new Table { Id = 2, TableNumber = "T2", Capacity = 4, Location = "Indoor", IsAvailable = true },
 //    new Table { Id = 3, TableNumber = "T3", Capacity = 6, Location = "Outdoor", IsAvailable = true },
 //    new Table { Id = 4, TableNumber = "T4", Capacity = 4, Location = "Window Side",
 //);


        }




    }
}
