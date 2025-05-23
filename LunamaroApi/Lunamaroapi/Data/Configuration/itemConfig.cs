using Microsoft.EntityFrameworkCore;
using Lunamaroapi.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Lunamaroapi.Data.Configuration
{
    public class itemConfig : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

         
         
            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(i => i.Description)
              .HasMaxLength(500) // Optional: Limit string length
              .IsRequired(false); // Optional: if description is not required

            // Price
            builder.Property(i => i.Price)
                   .HasColumnType("decimal(18,2)") // Ensures accuracy in DB
                   .IsRequired(); // Optional: make sure it's not null

            builder.HasOne(i => i.Category)
                   .WithMany(c => c.Items)
                   .HasForeignKey(i => i.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
