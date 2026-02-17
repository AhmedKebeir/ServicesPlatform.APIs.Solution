using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.TechDept_Specs
{
    public class TechDeptsSpceifications:BaseSpecifications<Technicians_Departments>
    {
        public TechDeptsSpceifications(int techId)
            :base(ud=>ud.TechnicianId ==techId)
        {
            Includes.Add(d => d.Department);
        }
    }
}
