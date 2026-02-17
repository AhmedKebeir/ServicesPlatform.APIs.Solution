using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnable { get; set; }
        public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> ThenIncludes { get; set; } = new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>>();

        public BaseSpecifications()
        {

        }
        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;

        }

        protected void AddThenInclude<TPreviousProperty>(
    Expression<Func<T, IEnumerable<TPreviousProperty>>> previous,
    Expression<Func<TPreviousProperty, object>> thenInclude)
        {
            ThenIncludes.Add(query => query.Include(previous).ThenInclude(thenInclude));
        }
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }


        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnable = true;
            Skip = skip;
            Take = take;
        }
    }
}
