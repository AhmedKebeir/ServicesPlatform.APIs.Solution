using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class GetReviewByOrderIdAndTechnicianSpecifications : BaseSpecifications<Review>
    {
        public GetReviewByOrderIdAndTechnicianSpecifications(int orderId)
            : base(r => r.OrderId == orderId && r.AuthorType == ReviewAuthorType.Technician)
        {
        }

    }
}
