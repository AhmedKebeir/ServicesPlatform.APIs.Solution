using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Review_Specs
{
    public class GetAllReviewsSpceifications : BaseSpecifications<Review>
    {
        public GetAllReviewsSpceifications(ReviewsSepcParams specParams, ReviewAuthorType? reviewAuthor = null)
            : base(r => (!specParams.OrderId.HasValue || r.OrderId == specParams.OrderId) &&
            (string.IsNullOrEmpty(specParams.UserId) || r.UserId == specParams.UserId) &&
            (!specParams.TechnicianId.HasValue || r.TechnicianId == specParams.TechnicianId) &&
                        (!reviewAuthor.HasValue || r.AuthorType == reviewAuthor.Value)
            )
        {

            AddOrderByDesc(r => r.Date);
            Includes.Add(r => r.User);
            Includes.Add(r => r.Technician.User);
            Includes.Add(r => r.Order);

            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);


        }
    }
}
