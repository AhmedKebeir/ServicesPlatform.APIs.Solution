using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Order_Images
    {
        public string ImageUrl { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }//finish

    }
}
