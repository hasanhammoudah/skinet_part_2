using Core.Entities.OrderAggreate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShippingAddress, a =>

                a.WithOwner()
            );

            builder.OwnsOne(o => o.PaymentSummary, a =>

               a.WithOwner()
           );
            builder.Property(o => o.Status).HasConversion(
             o => o.ToString(),
             o => (OrderStatus)Enum.Parse(typeof(OrderStatus), o)
            );
            builder.Property(x => x.Subtotal).HasColumnType("decimal(18,2)");
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
            builder.Property(x=>x.OrderDate).HasConversion(o=>o.ToUniversalTime(),o=>DateTime.SpecifyKind(o, DateTimeKind.Utc));
        }
    }
}