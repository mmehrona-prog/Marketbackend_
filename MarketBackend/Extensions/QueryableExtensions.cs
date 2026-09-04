using System.Linq;
namespace MarketBackend.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var skipAmount= (pageNumber - 1) * pageSize;
            return query.Skip(skipAmount).Take(pageSize);
        }
    }
}
