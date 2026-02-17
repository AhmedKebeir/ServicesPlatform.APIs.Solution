using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Department_Specs
{
    public class GetDepartmentByNameSpecifications:BaseSpecifications<Department>
    {
        public GetDepartmentByNameSpecifications(string name)
            :base(d=>d.Name==name)
        {
            
        }

    }
}
