using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class GetAllOrderByTechnicianIdSpecifications:BaseSpecifications<Order>
    {
        public GetAllOrderByTechnicianIdSpecifications(int technicianId)
            : base(o => o.TechnicianId == technicianId)
        {
            Includes.Add(o =>o.User);
            Includes.Add(o => o.Department);
            Includes.Add(o => o.Images);
            Includes.Add(o => o.Review);
            Includes.Add(o => o.Address);
            AddOrderByDesc(o=>o.CreateAt);


        }
        public GetAllOrderByTechnicianIdSpecifications(string userId)
            : base(o => o.UserId == userId)
        {
            Includes.Add(o=>o.Technician.User);
            Includes.Add(o => o.Department);
            Includes.Add(o => o.Images);
            Includes.Add(o => o.Review);
            Includes.Add(o => o.Address);
            AddOrderByDesc(o => o.CreateAt);
        }

      
    }
}
