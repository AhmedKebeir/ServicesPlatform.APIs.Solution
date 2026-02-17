using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class GetOrderByIdSpecifications:BaseSpecifications<Order>
    {
        public GetOrderByIdSpecifications(int id)
            :base(o=>o.Id == id)
        {
            Includes.Add(o => o.Technician.User);
            Includes.Add(o => o.Technician);
            Includes.Add(o => o.User);
            Includes.Add(o => o.Department);
            Includes.Add(o => o.Images);
            Includes.Add(o => o.Review);
            Includes.Add(o => o.Address);
        }
    }
}
