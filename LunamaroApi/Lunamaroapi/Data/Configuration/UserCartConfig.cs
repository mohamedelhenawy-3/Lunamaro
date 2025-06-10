using Lunamaroapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lunamaroapi.Data.Configuration
{
    public class UserCartConfig : IEntityTypeConfiguration<UserCart>
    {
        public void Configure(EntityTypeBuilder<UserCart> builder)
        {
            builder.HasKey(x => x.Id);


            builder.Property(x => x.Quantity).IsRequired();

            builder.HasOne(x => x.User).WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Item)
              .WithMany()
              .HasForeignKey(x => x.ItemId)
              .OnDelete(DeleteBehavior.Cascade);
            builder.ToTable("UserCarts");

        }
    }
}
