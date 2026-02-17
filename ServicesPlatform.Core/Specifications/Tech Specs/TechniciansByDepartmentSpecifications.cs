using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Tech_Specs
{
    public class TechniciansByDepartmentSpecifications:BaseSpecifications<Technician>
    {
        public TechniciansByDepartmentSpecifications(int departmentId)
            :base(t=>t.Departments.Any(d=>d.DepartmentId == departmentId))
        {
            Includes.Add(t => t.User);
            Includes.Add(t=>t.User.Addresses);

        }
    }
}
