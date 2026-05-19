using Lunamaroapi.Data.Configuration;
using Lunamaroapi.Models;
using Lunamaroapi.Models.Cart;
using Lunamaroapi.Models.ItemsModel;
using Lunamaroapi.Models.Offers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Data
{
    public class AppDBContext :IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<AddOnReward> AddOnRewards { get; set; }
        public DbSet<DiscountTier> DiscountTiers { get; set; }
        public DbSet<WeeklyDeal>  WeeklyDeals { get; set; }
        public DbSet<Item> Items { get; set; }
      public   DbSet<Category> Categories { get; set; }
        public DbSet<UserCart> UserCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserOrderHeader> UserOrderHeaders { get; set; }

        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CategoryRelationship> categoryRelationships { get; set; }
        public DbSet<UserCartAddOn> userCartAddOns { get; set; }
        public DbSet<ItemAddOn> ItemAddOns { get; set; }
        public DbSet<ItemRelationship> ItemRelationships { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // important for Identity tables

            modelBuilder.Entity<CategoryRelationship>()
        .HasOne(r => r.Category)
        .WithMany()
        .HasForeignKey(r => r.CategoryId)
        .OnDelete(DeleteBehavior.NoAction); // ← no cascade

            modelBuilder.Entity<ItemRelationship>()
                 .HasIndex(r => r.ItemId);      

            modelBuilder.Entity<ItemRelationship>()
                .HasIndex(r => r.RelatedItemId);
          
            modelBuilder.Entity<UserCart>()
                .HasIndex(c => c.UserId)      
                .HasDatabaseName("IX_UserCarts_UserId");

            modelBuilder.Entity<UserCart>()
                .HasIndex(c => c.ItemId)       
                .HasDatabaseName("IX_UserCarts_ItemId");

            modelBuilder.Entity<UserCartAddOn>()
                .HasIndex(a => a.UserCartId)   
                .HasDatabaseName("IX_UserCartAddOns_UserCartId");

            modelBuilder.Entity<ItemAddOn>()
                .HasIndex(a => a.ItemId)       
                .HasDatabaseName("IX_ItemAddOns_ItemId");





            modelBuilder.Entity<UserCartAddOn>()
                .HasIndex(u => u.UserCartId);
            modelBuilder.Entity<CategoryRelationship>()
                .HasOne(r => r.RelatedCategory)
                .WithMany()
                .HasForeignKey(r => r.RelatedCategoryId)
                .OnDelete(DeleteBehavior.NoAction); // ← no cascade

            // Fix same issue on ItemRelationships (same problem, two FKs to Items)
            modelBuilder.Entity<ItemRelationship>()
                .HasOne(r => r.Item)
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ItemRelationship>()
                .HasOne(r => r.RelatedItem)
                .WithMany()
                .HasForeignKey(r => r.RelatedItemId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserCartAddOn>()
    .HasOne(u => u.UserCart)
    .WithMany(c => c.AddOns)
    .HasForeignKey(u => u.UserCartId)
    .OnDelete(DeleteBehavior.NoAction); // 🔥 FIX HERE

            modelBuilder.Entity<UserCartAddOn>()
                .HasOne(u => u.AddOn)
                .WithMany()
                .HasForeignKey(u => u.AddOnId)
                .OnDelete(DeleteBehavior.Cascade); // OK
              



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
