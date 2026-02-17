using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class GetAllReviewsByTechnicianIdSpecifications:BaseSpecifications<Review>
    {
        public GetAllReviewsByTechnicianIdSpecifications(int id)
            : base(r => r.TechnicianId ==id)
        {
            
        }
    }
}
