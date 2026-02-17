using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.TechDept_Specs
{
    public class TechDeptSpceifications:BaseSpecifications<Technicians_Departments>
    {
        public TechDeptSpceifications(int techId,int deptId)
            :base(td=>td.TechnicianId==techId&&td.DepartmentId==deptId)
        {
            Includes.Add(d => d.Department);
        }
    }
}
