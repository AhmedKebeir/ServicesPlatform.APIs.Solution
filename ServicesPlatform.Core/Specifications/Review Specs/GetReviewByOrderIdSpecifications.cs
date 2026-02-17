using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class GetReviewByOrderIdSpecifications : BaseSpecifications<Review>
    {
        public GetReviewByOrderIdSpecifications(int orderId ,ReviewAuthorType reviewAuthor)
            : base(r => r.OrderId == orderId && r.AuthorType == reviewAuthor)
        {
        }
    }
}
