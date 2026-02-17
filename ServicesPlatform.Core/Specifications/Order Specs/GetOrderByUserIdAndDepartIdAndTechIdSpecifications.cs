using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class GetOrderByUserIdAndDepartIdAndTechIdSpecifications: BaseSpecifications<Order>
    {
        public GetOrderByUserIdAndDepartIdAndTechIdSpecifications(string userId, int departmentId, int technicianId)
            : base(o => o.UserId == userId && o.DepartmentId == departmentId && o.TechnicianId == technicianId)
        {
          
        }
    }
}
