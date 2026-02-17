using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Department_Specs
{
    public class GetDepartmentsWithPaginationSpecifications:BaseSpecifications<Department>
    {
        public GetDepartmentsWithPaginationSpecifications(DepartmentSpecParams specParams)
            :base(d=>
            string.IsNullOrEmpty(specParams.Search) || d.Name.ToLower().Contains(specParams.Search)
              
            )
        {
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "orderAsc":
                        AddOrderBy(d=>d.Orders.Count);
                        break;
                    case "orderDesc":
                        AddOrderByDesc(P => P.Orders.Count);
                        break;

                    default:
                        AddOrderByDesc(P => P.Orders.Count);
                        break;
                }
            }
            else
                AddOrderBy(d => d.Name);

            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
        }
    }
}
