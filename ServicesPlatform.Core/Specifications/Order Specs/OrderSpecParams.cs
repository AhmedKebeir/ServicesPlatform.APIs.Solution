using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class OrderSpecParams
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

        public int? DepartmentId { get; set; }
        public string? UserId { get; set; }
        public int? TechnicianId { get; set; }
        public DateOnly? Date {  get; set; }

        public OrderStatus? Status { get; set; }

        public string? Activity { get; set; }

        public string? City { get; set; }

        public string? Center { get; set; }


        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
    }
}
