using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class OrderAddress
    {
        public OrderAddress()
        {
            
        }
        public OrderAddress(string city,string center,string street)
        {
            City= city;
            Center= center;
            Street= street;
        }
        public string City { get; set; }
        public string Center { get; set; }
        public string Street { get; set; }
    }
}
