using Core.Entities;
using Core.Entities.OrderAggreate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;


namespace API.Controllers
{
    public class PaymentsController(IPaymentService paymentService,
    IUnitOfWork unit , ILogger<PaymentsController> logger,IConfiguration config) : BaseApiController
    {
        private readonly string _whSecret = config["StripeSettings:WhSecret"]!;
        [Authorize]
        [HttpPost("{cartId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);
            if(cart == null) return BadRequest("Problem with your cart");
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
    
    try
    {
        var stripeEvent = ConstructStripeEvent(json);
        logger.LogInformation("✅ Stripe event constructed successfully");

        if (stripeEvent.Data.Object is not PaymentIntent intent)
        {
            logger.LogWarning("⚠️ Event is not a PaymentIntent");
            return BadRequest("Invalid event data");
        }

        logger.LogInformation("✅ Handling PaymentIntent: {IntentId}", intent.Id);
        await HandlePaymentIntentSucceeded(intent);

        return Ok();
    }
    catch (StripeException ex)
    {
        logger.LogError(ex, "❌ StripeException: invalid signature or event");
        return StatusCode(StatusCodes.Status500InternalServerError, "Stripe webhook error");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ General Exception in webhook handler");
        return StatusCode(StatusCodes.Status500InternalServerError, "Unhandled error in webhook");
    }
}

      private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
{
    logger.LogInformation("✅ Webhook received for PaymentIntent: {IntentId}", intent.Id);

    if (intent.Status == "succeeded")
    {
        var spec = new OrderSpecification(intent.Id, true);
        var order = await unit.Repository<Core.Entities.OrderAggreate.Order>().GetEntityWithSpec(spec);

        if (order == null)
        {
            logger.LogError("❌ Order not found for PaymentIntent ID: {IntentId}", intent.Id);
            return;
        }

        var expectedAmount = (long)order.GetTotal() * 100;

        if (expectedAmount != intent.Amount)
        {
            order.Status = OrderStatus.PaymentMismatch;
            logger.LogWarning("⚠️ Payment mismatch. Expected: {Expected}, Actual: {Actual}", expectedAmount, intent.Amount);
        }
        else
        {
            order.Status = OrderStatus.PaymentReceived;
            logger.LogInformation("💰 Payment received and matched.");
        }

        await unit.Complete();
    }
}


        private Event ConstructStripeEvent(string json)
        {
           
            try
            {
               return EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], _whSecret);
               
            }
            catch(Exception ex)
            {
                logger.LogError(ex, "Stripe webhook error");
                throw new StripeException("Invalid signature");
            }
         
        }

    }
    
}