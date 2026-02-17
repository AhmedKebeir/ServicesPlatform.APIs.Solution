using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Users_Specs
{
    public class UsersSpecParams
    {
        private const int MaxPageSize = 10;
        private int pageSize =10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }

        public int PageIndex { get; set; } = 1;


        public string? Sort { get; set; }

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
