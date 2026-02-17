using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Technicians_Departments:BaseEntity
    {
        public int TechnicianId { get; set; }
        public Technician Technician { get; set; }//finish
        public int DepartmentId { get; set; }
        public Department Department { get; set; }//finish
    }
}
