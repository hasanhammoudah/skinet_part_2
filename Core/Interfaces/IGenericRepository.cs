using Core.Entities;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        /// إرجاع عنصر واحد حسب المعرف
        Task<T?> GetByIdAsync(int id);
        /// إرجاع جميع العناصر من الجدول
        Task<IReadOnlyList<T>> ListAllAsync();
        /// إرجاع عنصر واحد باستخدام شروط محددة (Specification)
        Task<T?> GetEntityWithSpec(ISpecification<T> spec);
        /// إرجاع قائمة من العناصر بناءً على شروط الـ Specification
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        /// إرجاع عنصر واحد وتحويله إلى شكل معين (مثل DTO) باستخدام Specification
        Task<TResult?> GetEntityWithSpec<TResult>(ISpecification<T, TResult> spec);
        /// إرجاع قائمة من العناصر المحوّلة (DTOs) باستخدام Specification
        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T, TResult> spec);
        /// إضافة كيان جديد لقاعدة البيانات
        void Add(T entity);
        /// تعديل كيان موجود

        void Update(T entity);
        /// حذف كيان من قاعدة البيانات

        void Remove(T entity);

        /// التحقق من وجود كيان حسب الـ Id

        bool Exists(int id);
        /// عدّ عدد العناصر التي تطابق شروط معينة باستخدام Specification
        Task<int> CountAsync(ISpecification<T> spec);
    }
}