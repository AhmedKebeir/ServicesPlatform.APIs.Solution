using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class GetAllReviewForUserSpceifications:BaseSpecifications<Review>
    {
        public GetAllReviewForUserSpceifications(string userId)
            : base(r => r.UserId == userId)
        {
            AllIncludes();

        }
        public GetAllReviewForUserSpceifications(int technicianId)
            : base(r => r.TechnicianId == technicianId)
        {
            AllIncludes();
        }
        private void AllIncludes()
        {
            Includes.Add(r => r.User);
            Includes.Add(r => r.Technician);
            Includes.Add(r => r.Order);
        }
    }
}
