using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Tech_Specs
{
    public class GetTechnicianByIdSepcifications:BaseSpecifications<Technician>
    {
        public GetTechnicianByIdSepcifications(string id)
            :base(t=>t.UserId==id)
        {
            Includes.Add(t => t.User);
            Includes.Add(t => t.Departments);
            Includes.Add(t => t.Reviews);
            Includes.Add(t => t.User.Addresses);
            Includes.Add(t => t.Order);

        }
    }
}
