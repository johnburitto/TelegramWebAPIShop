using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    internal class ThumbnailConfiguration : IEntityTypeConfiguration<Thumbnail>
    {
        public void Configure(EntityTypeBuilder<Thumbnail> builder)
        {
            builder.Property(thumbnail => thumbnail.Id)
                .UseIdentityColumn()
                .IsRequired();

            builder.Property(thumbnail => thumbnail.URI)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasOne(thumbnail => thumbnail.Product)
                .WithMany(product => product.Thumbnails)
                .HasForeignKey(thumbnail => thumbnail.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Thumbnails_ProductId");
        }
    }
}
