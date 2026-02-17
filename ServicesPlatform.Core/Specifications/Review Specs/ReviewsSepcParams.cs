using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class ReviewsSepcParams
    {

        private const int MaxPageSize = 10;
        private int pageSize = 4;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public int PageIndex { get; set; } = 1;

        public string? Sort { get; set; }

        public int? OrderId { get; set; }
        public string? UserId { get; set; }
        public int? TechnicianId { get; set; }


     






    }
}
