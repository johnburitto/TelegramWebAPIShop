using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(product => product.Id)
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(product => product.Name)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(product => product.Description)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(product => product.Price)
                .IsRequired();
        }
    }
}
