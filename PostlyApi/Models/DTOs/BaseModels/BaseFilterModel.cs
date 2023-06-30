using System.Linq.Expressions;

namespace PostlyApi.Models.DTOs.BaseModels
{
    public abstract class BaseFilterModel<T>
    {
        public virtual Expression<Func<T, bool>> GetExpression() => _ => true;
        public virtual IQueryable<T> GetMatches(IEnumerable<T> source) => GetMatches(source.AsQueryable());
        public virtual IQueryable<T> GetMatches(IQueryable<T> source) => source.Where(GetExpression());
    }
}
