using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Helpers
{
    public static class IQueryableExtensions
    {
        public static PagedList<T> ToPaginatedList<T>(this IQueryable<T> source, ResourceParameters resourceParameters)
        {
            var pageNumber = resourceParameters.PageNumber;
            var pageSize = resourceParameters.PageSize;

            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
