using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggreate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{
    public class PaymentsController(
        IPaymentService paymentService,
        IUnitOfWork unit,
        ILogger<PaymentsController> logger,
        IConfiguration config,
        IHubContext<NotificationHub> hubContext
    ) : BaseApiController
    {
        private readonly string _whSecret = config["StripeSettings:WhSecret"]!;

        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
            if (cart == null) return BadRequest("Problem with your cart");
            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await unit.Repository<DeliveryMethod>().ListAllAsync());
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> StripeWebhook()
        {
            logger.LogInformation("✅ Stripe webhook endpoint hit");

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            logger.LogInformation("📥 Raw Stripe JSON: {json}", json);
            logger.LogInformation("📦 Stripe-Signature Header: {sig}", stripeSignature);

            try
            {
                // 👇 هذه الإضافة تحل مشكلة إصدار Stripe API
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _whSecret,
                    throwOnApiVersionMismatch: false
                );

                logger.LogInformation("📌 Stripe event type: {type}", stripeEvent.Type);

                if (stripeEvent.Type == "payment_intent.succeeded")
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    if (intent == null)
                    {
                        logger.LogWarning("❌ Could not extract PaymentIntent from event.");
                        return BadRequest("Invalid event data");
                    }

                    logger.LogInformation("✅ Handling PaymentIntent with ID: {id}", intent.Id);
                    await HandlePaymentIntentSucceeded(intent);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "❌ StripeException: Signature or event invalid");
                return StatusCode(500, $"StripeException: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ General exception occurred");
                return StatusCode(500, $"General error: {ex.Message}");
            }
        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            logger.LogInformation("✅ PaymentIntent succeeded: {IntentId}", intent.Id);

            var spec = new OrderSpecification(intent.Id, true);
                      var order = await unit.Repository<Core.Entities.OrderAggreate.Order>().GetEntityWithSpec(spec)
                ?? throw new Exception("Order not found");


            if (order == null)
            {
                logger.LogError("❌ Order not found for intent ID: {IntentId}", intent.Id);
                return;
            }

            var expectedAmount = (long)Math.Round(order.GetTotal() * 100, MidpointRounding.AwayFromZero);
            if (expectedAmount != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
                logger.LogWarning("⚠️ Payment amount mismatch. Expected: {Expected}, Actual: {Actual}", expectedAmount, intent.Amount);
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
                logger.LogInformation("💵 Payment matched and marked as received.");
            }

            await unit.Complete();
            var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
            if(!string.IsNullOrEmpty(connectionId))
            {
                await hubContext.Clients.Client(connectionId).SendAsync("OrderCompleteNotification", order.ToDto());
            }
            else
            {
                logger.LogWarning("❌ No connection found for email: {Email}", order.BuyerEmail);
            }
        }
    }
}
