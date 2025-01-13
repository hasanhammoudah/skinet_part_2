using Core.Entities;
using Core.Sepecification;

namespace Core.Specification
{
    public class TypeListSpecification : BaseSpecification<Product,string>
    {
          public TypeListSpecification()
        {
            AddSelect(x=>x.Type);
            ApplyDistinct();
        }
    }
}