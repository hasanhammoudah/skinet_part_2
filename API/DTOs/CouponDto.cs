namespace API.DTOs;

public class CouponDto
{
    public string Code { get; set; }
    public double? PercentOff { get; set; }
    public double? AmountOff { get; set; }
    public DateTime? Expiration { get; set; }
}
