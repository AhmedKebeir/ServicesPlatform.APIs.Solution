using Microsoft.EntityFrameworkCore;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Repositories
{
    internal static class SpecificationsEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
        {
            var query = inputQuery;

            // 1️⃣ Apply Criteria
            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            // 2️⃣ Apply Includes (VERY IMPORTANT)
            query = spec.Includes
                .Aggregate(query, (current, includeExpression) => current.Include(includeExpression));

            query = spec.ThenIncludes.Aggregate(query,
                    (current, thenInclude) => thenInclude(current));

            // 3️⃣ Apply Sorting
            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDesc is not null)
                query = query.OrderByDescending(spec.OrderByDesc);

            // 4️⃣ Apply Pagination in the LAST step
            if (spec.IsPaginationEnable)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }
    }

}
