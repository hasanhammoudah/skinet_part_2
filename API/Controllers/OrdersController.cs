using API.DTOs;
using API.Extentions;
using Core.Entities;
using Core.Entities.OrderAggreate;
using Core.Interfaces;
using Core.Specification;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrdersController(ICartService cartService,IUnitOfWork unit) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var email = User.GetEmail();
            var cart = await cartService.GetCartAsync(orderDto.CartId);
            if(cart == null) return BadRequest("Cart not found");
            if(cart.PaymentIntentId == null) return BadRequest("Payment intent for this order");

            var items = new List<OrderItem>();
            foreach(var item in cart.Items){
                var productItem = await unit.Repository<Product>().GetByIdAsync(item.ProductId);
                if(productItem == null) return BadRequest("Problem with the order");
                var itemOrdered = new ProductItemOrdered{
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    PictureUrl = item.PictureUrl
                };
                var orderItem = new OrderItem{
                    ItemOrdered = itemOrdered,
                    Price = productItem.Price,
                    Quantity = item.Quantity
                };
                items.Add(orderItem);
            }
            var deliveryMethod = await unit.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if(deliveryMethod == null) return BadRequest("No delivery method selected");
            var order = new Order{
                BuyerEmail = email,
                ShippingAddress = orderDto.ShippingAddress,
                DeliveryMethod = deliveryMethod,
                PaymentSummary = orderDto.PaymentSummary,
                PaymentIntentId = cart.PaymentIntentId,
                OrderItems = items,
                Subtotal = items.Sum(i => i.Price * i.Quantity)
            };
           unit.Repository<Order>().Add(order);
           if(await unit.Complete()){
               return Ok(order);
           }
           return BadRequest("Problem creating order");
        }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrdersForUser()
    {
        var email = User.GetEmail();
        var spec = new OrderSpecification(email);
        var orders = await unit.Repository<Order>().ListAsync(spec);
        return Ok(orders);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> GetOrderById(int id)
    {
        var email = User.GetEmail();
        var spec = new OrderSpecification(email,id);
        var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
        if(order == null) return NotFound();
        return order;
    }
}
}