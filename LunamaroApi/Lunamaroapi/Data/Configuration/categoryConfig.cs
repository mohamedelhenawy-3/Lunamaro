using Lunamaroapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lunamaroapi.Data.Configuration
{
    public class categoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");



            builder.HasKey(i => i.Id);

            // Properties
            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasMany(i => i.Items)
                           .WithOne(c => c.Category)
                           .HasForeignKey(i => i.CategoryId) 
                           .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
