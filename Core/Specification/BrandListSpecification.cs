using System.Linq.Expressions;
using Core.Entities;
using Core.Sepecification;

namespace Core.Specification
{
    public class BrandListSpecification : BaseSpecification<Product, string>
    {
        public BrandListSpecification()
        {
            AddSelect(x=>x.Brand);
            ApplyDistinct();
        }
    }
}