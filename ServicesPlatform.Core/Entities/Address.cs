using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Address:BaseEntity
    {
        
        public string City { get; set; }
        public string Center { get; set; }
        public string Street { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }//finish
    }
}
