using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services
{
    public class PaymentService(IConfiguration config, ICartService cartService, IUnitOfWork unit) : IPaymentService
    {
        public async Task<ShoppingCart> CreateOrUpdatePaymentIntent(string cartId)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

            var cart = await cartService.GetCartAsync(cartId);
            if (cart == null) return null;

            var shippingPrice = 0m;
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
                if (deliveryMethod == null) return null;
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in cart.Items)
            {
                var productItem = await unit.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);
                if (productItem == null) return null;
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var subtotal = cart.Items.Sum(i => i.Quantity * i.Price);
            decimal discount = 0;

            if (cart.Coupon != null)
            {
                if (cart.Coupon.AmountOff.HasValue)
                {
                    discount = (decimal)cart.Coupon.AmountOff.Value;
                }
                else if (cart.Coupon.PercentOff.HasValue)
                {
                discount = subtotal * ((decimal)cart.Coupon.PercentOff.Value / 100m); 
                }
            }

            var total = subtotal + shippingPrice - discount;

            var service = new PaymentIntentService();
            PaymentIntent? intent = null;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(total * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var currentIntent = await service.GetAsync(cart.PaymentIntentId);
                if (currentIntent.Status == "requires_payment_method" || currentIntent.Status == "requires_confirmation" || currentIntent.Status == "requires_action")
                {
                    var options = new PaymentIntentUpdateOptions
                    {
                        Amount = (long)(total * 100)
                    };
                    intent = await service.UpdateAsync(cart.PaymentIntentId, options);
                }
                else
                {
                    var options = new PaymentIntentCreateOptions
                    {
                        Amount = (long)(total * 100),
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };
                    intent = await service.CreateAsync(options);
                    cart.PaymentIntentId = intent.Id;
                    cart.ClientSecret = intent.ClientSecret;
                }
            }

            await cartService.SetCartAsync(cart);
            return cart;
        }
    }
}
