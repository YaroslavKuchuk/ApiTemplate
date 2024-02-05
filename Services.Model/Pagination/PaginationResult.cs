using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Services.Model.Pagination
{
    public class PaginationResult<TModel> where TModel : class
    {
        public List<TModel> Items { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public PaginationResult()
        {
        }

        public PaginationResult(int page, int pageSize, int totalCount, int totalPages, List<TModel> items)
        {
            var itemsList = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
            Items = itemsList;
        }

        public PaginationResult(int page, int pageSize, IQueryable<object> query)
        {
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var itemsList = Mapper.Map<List<TModel>>(query.Skip((page - 1) * pageSize).Take(pageSize).ToList());
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
            Items = itemsList;
        }
    }
}
