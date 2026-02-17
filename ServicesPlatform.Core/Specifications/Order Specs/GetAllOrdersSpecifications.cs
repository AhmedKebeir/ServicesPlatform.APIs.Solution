using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class GetAllOrdersSpecifications:BaseSpecifications<Order>
    {
        public GetAllOrdersSpecifications()
            :base()
        {
            Includes.Add(o => o.Technician.User);
            Includes.Add(o => o.Department);
            Includes.Add(o => o.Images);
            Includes.Add(o => o.Review);
            Includes.Add(o => o.Address);
            AddOrderByDesc(o => o.CreateAt);
        }

        public GetAllOrdersSpecifications(OrderSpecParams specParams)
            :base(o=>
                    (!specParams.TechnicianId.HasValue || o.TechnicianId==specParams.TechnicianId.Value)&&
                    (string.IsNullOrEmpty(specParams.UserId) || o.UserId==specParams.UserId)&&
                    (!specParams.DepartmentId.HasValue || o.DepartmentId==specParams.DepartmentId.Value)&&
                    (!specParams.Date.HasValue || DateOnly.FromDateTime(o.CreateAt.DateTime) == specParams.Date.Value)&&
                    (string.IsNullOrEmpty(specParams.Status.ToString())||  o.Status==specParams.Status)

                    
                 
                 )
        {
            Includes.Add(o => o.Technician.User);
            Includes.Add(o => o.User);
            Includes.Add(o => o.Department);
            Includes.Add(o => o.Images);
            Includes.Add(o => o.Review);
            Includes.Add(o => o.Address);
            AddOrderByDesc(o => o.CreateAt);


            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);
        }
    }
}
