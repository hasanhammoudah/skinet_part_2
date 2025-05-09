using System.Linq.Expressions;
using Core.Entities.OrderAggreate;
using Core.Sepecification;

namespace Core.Specification
{
  //TODO 
    public class OrderSpecification : BaseSpecification<Order>
    {

        public OrderSpecification(string email) : base(o => o.BuyerEmail == email)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            AddOrderByDescending(x => x.OrderDate);
        }
          public OrderSpecification(string email,int id) : base(x => x.BuyerEmail == email && x.Id == id)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

        public OrderSpecification(string paymentIntentId,bool isPaymentIntent) : base(o => o.PaymentIntentId == paymentIntentId)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
        }
    }
}