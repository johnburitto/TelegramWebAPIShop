using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.Property(discount => discount.Id)
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(discount => discount.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(discount => discount.Description)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(discount => discount.DiscoutInPercent)
                .IsRequired();

            builder.Property(discount => discount.NormalizedDiscount)
                .IsRequired();

            builder.Property(discount => discount.StartDate)
                .IsRequired();

            builder.Property(discount => discount.EndDate)
                .IsRequired();

            builder.Property(discount => discount.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasMany(discount => discount.Products)
                .WithMany(product => product.Discounts)
                .UsingEntity(join => join.ToTable("ProductsWithDiscounts"));
        }
    }
}
