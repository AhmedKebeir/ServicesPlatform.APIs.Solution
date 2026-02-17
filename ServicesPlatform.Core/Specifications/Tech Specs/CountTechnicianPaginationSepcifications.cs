using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Tech_Specs
{
    public class CountTechnicianPaginationSepcifications : BaseSpecifications<Technician>
    {
        public CountTechnicianPaginationSepcifications(TechniciansSpecParams specParams)
            : base(t =>
            (string.IsNullOrEmpty(specParams.Search) || t.User.DisplayName.ToLower().Contains(specParams.Search)) &&

            (string.IsNullOrEmpty(specParams.City)
                 || t.User.Addresses.Any(a => a.City.ToLower().Contains(specParams.City.ToLower()))) &&

            (string.IsNullOrEmpty(specParams.Center)
                     || t.User.Addresses.Any(a => a.Center.ToLower().Contains(specParams.Center.ToLower()))) &&

            (!specParams.IsActive.HasValue || t.IsActive == specParams.IsActive.Value) &&

            (!specParams.DepartmentId.HasValue || t.Departments.Any(d => d.DepartmentId == specParams.DepartmentId))




            )
        {

        }
    }
}
