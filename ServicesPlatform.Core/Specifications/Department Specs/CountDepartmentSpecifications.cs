using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Department_Specs
{
    public class CountDepartmentSpecifications:BaseSpecifications<Department>
    {
        public CountDepartmentSpecifications(DepartmentSpecParams specParams)
            :base(
                 d =>
            string.IsNullOrEmpty(specParams.Search) || d.Name.ToLower().Contains(specParams.Search))
        {
            
        }
    }
}
