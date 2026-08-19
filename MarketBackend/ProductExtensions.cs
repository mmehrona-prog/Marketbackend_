using System;
using System.Linq;
using System.Linq.Expressions;
using MarketBackend.Models;

namespace MarketBackend
{
    public static class ProductExtensions
    {
        //метод расширения для фильтрации по критериям
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }
    }
}
