using Core.Entities.OrderAggreate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.ItemOrdered, o =>
            {
                o.WithOwner();
            });

            builder.Property(i => i.Price)
                .HasColumnType("decimal(18,2)");

            // builder.Property(i => i.Quantity)
            //     .IsRequired();
        }
    }
}