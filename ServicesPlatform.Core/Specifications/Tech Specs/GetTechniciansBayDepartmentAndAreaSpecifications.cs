using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Tech_Specs
{
    public class GetTechniciansBayDepartmentAndAreaSpecifications : BaseSpecifications<Technician>
    {
        public GetTechniciansBayDepartmentAndAreaSpecifications(int deptId,string city,string center)
            :base(t=>
            t.Departments.Any(d => d.DepartmentId == deptId) &&
            t.User.Addresses.Any(a=>a.City==city && a.Center ==center))
        {
            Includes.Add(t => t.User);
            Includes.Add(t => t.Departments);
            Includes.Add(t=> t.User.Addresses);
        }
    }
}
