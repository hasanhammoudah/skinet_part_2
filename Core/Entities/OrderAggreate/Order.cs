using Core.Interfaces;

namespace Core.Entities.OrderAggreate
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public required string BuyerEmail { get; set; }

        public ShippingAddress ShippingAddress { get; set; } = null!;

        public DeliveryMethod DeliveryMethod { get; set; } = null!;

        public PaymentSummary PaymentSummary { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = [];

        public decimal Subtotal { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public required string PaymentIntentId { get; set; }

        public AppCoupon? Coupon { get; set; } // الكوبون المستخدم في الطلب (اختياري)
        public decimal Discount { get; set; } // قيمة الخصم الناتجة عن الكوبون
        public decimal Total { get; set; } // المبلغ النهائي بعد الخصم والشحن


        public decimal GetTotal()
        {
         return Subtotal + DeliveryMethod.Price - (Discount > 0 ? Discount : 0);
        }

    }
}