using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace XtraSpurt.XRepository
{
    public static class XIQueryablePagedExtensions
    {
        public static XPagedResult<TM, TV> GetPaged<TM, TV>(this IQueryable<TM> query,
            XPagedQueryParam param, IMapper mapper) where TV : class where TM : class
        {
            var result = new XPagedResult<TM, TV> { Current = param.Page,Size = param.Size, RowCount = query.Count() };

            var pageCount = (double)result.RowCount / param.Size;
            result.Count = (int)Math.Ceiling(pageCount);

            var skip = (param.Page - 1) * param.Size;
            var returnlist = query.Skip(skip).Take(param.Size).ToList();
            result.Results = mapper.Map<List<TM>, List<TV>>(returnlist);

            return result;
        }

        public static XPagedResult<TM> GetPaged<TM>(this IQueryable<TM> query,
            XPagedQueryParam param) where TM : class
        {
            var result = new XPagedResult<TM> { Current = param.Page, Size = param.Size, RowCount = query.Count() };

            var pageCount = (double)result.RowCount / param.Size;
            result.Count = (int)Math.Ceiling(pageCount);

            var skip = (param.Page - 1) * param.Size;
            var returnlist = query.Skip(skip).Take(param.Size).ToList();
            result.Results = returnlist;

            return result;
        }
    }
}