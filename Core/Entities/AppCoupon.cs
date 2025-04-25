namespace Core.Entities;

public class AppCoupon
{
   public int Id { get; set; } // ✅ المفتاح الأساسي

    public required string Name { get; set; }
    public decimal? AmountOff { get; set; }
    public decimal? PercentOff { get; set; }
    public required string PromotionCode { get; set; }
    public required string CouponId { get; set; }
}