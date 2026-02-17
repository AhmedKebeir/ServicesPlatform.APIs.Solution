using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class CountReviewsWithSpecParams : BaseSpecifications<Review>
    {
        public CountReviewsWithSpecParams(ReviewsSepcParams specParams, ReviewAuthorType? reviewAuthor = null)
      : base(r => (!specParams.OrderId.HasValue || r.OrderId == specParams.OrderId) &&
      (string.IsNullOrEmpty(specParams.UserId) || r.UserId == specParams.UserId) &&
      (!specParams.TechnicianId.HasValue || r.TechnicianId == specParams.TechnicianId) &&
      (!reviewAuthor.HasValue || r.AuthorType == reviewAuthor.Value)
      )
        {




        }
    }
}
