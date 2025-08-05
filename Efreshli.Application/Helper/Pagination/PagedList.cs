using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Efreshli.Application.Helper.Pagination
{
    public class PagedList<T> : List<T>
    {
        public PaginationMetaData MetaData { get; private set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new PaginationMetaData(count, pageNumber, pageSize);

            AddRange(items);
        }
    }

    public class PaginationMetaData(int count, int pageNumber, int pageSize)
    {
        public int CurrentPage { get; private set; } = pageNumber;
        public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);
        public int PageSize { get; private set; } = pageSize;
        public int TotalCount { get; private set; } = count;

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
    public static class PaginationExtensions
    {
        public static async Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            var count = await source.CountAsync(cancellationToken: cancellationToken);

            return new PagedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
