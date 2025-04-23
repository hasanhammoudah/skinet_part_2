using System.Linq.Expressions;

namespace Core.Interfaces
{
    public interface ISpecification<T>
    {
        // شرط التصفية الأساسي مثل WHERE في SQL
        Expression<Func<T, bool>>? Criteria { get; }
        // ترتيب تصاعدي مثل ORDER BY
        Expression<Func<T, object>>? OrderBy { get; }
        // ترتيب تنازلي مثل ORDER BY DESC
        Expression<Func<T, object>>? OrderByDescending { get; }
        // العلاقات الملاحية اللي بدنا نعمل لها Include باستخدام Lambda
        List<Expression<Func<T, object>>> Includes { get; }
        // العلاقات الملاحية كـ string (لـ includes المعقدة المتداخلة)

        List<string> IncludeStrings { get; }
        // هل نريد إزالة التكرار من النتائج؟ (DISTINCT)

        bool IsDistinct { get; }
        // عدد العناصر اللي بدنا نأخذها (LIMIT)

        int Take { get; }
        // عدد العناصر اللي بدنا نتخطاها (OFFSET)

        int Skip { get; }
        // هل التصفح مفعل؟ (Skip + Take)

        bool IsPagingEnabled { get; }
        // تطبيق الشروط والفلاتر على الاستعلام
        IQueryable<T> ApplyCriteria(IQueryable<T> query);

    }

    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        // تحديد الحقول اللي بدنا نرجعها فقط من الكيان (SELECT)
        Expression<Func<T, TResult>>? Select { get; }

    }
}