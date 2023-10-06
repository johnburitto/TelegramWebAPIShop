using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(order => order.Id)
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(order => order.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(order => order.UserTelegramId)
                .HasMaxLength(12)
                .IsRequired();

            builder.Property(order => order.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(order => order.Phone)
                .HasMaxLength(13)
                .IsRequired();

            builder.Property(order => order.Address)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(order => order.TotalPrise)
                .IsRequired();

            builder.HasMany(order => order.Products)
                .WithMany(product => product.Orders)
                .UsingEntity(join => join.ToTable("OrderedProducts"));
        }
    }
}
