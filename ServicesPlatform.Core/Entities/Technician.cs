using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Technician : BaseEntity
    {
        public bool IsActive { get; set; }
        public int ExperienceYears { get; set; }
        public int? CountOfPoints { get; set; }
        public string? IdCard { get; set; }
        public string UserId { get; set; }

        public AppUser User { get; set; }//finish
        public ICollection<Technicians_Departments> Departments { get; set; } = new HashSet<Technicians_Departments>();//finish

        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();//finish
        public ICollection<Order> Order { get; set; } = new HashSet<Order>();//finish


        public double AverageRating
        {
            get
            {
                if (Reviews == null || !Reviews.Any())
                    return 0;

                var technicianReviews = Reviews
                    .Where(r => r.AuthorType == ReviewAuthorType.User);

                if (!technicianReviews.Any())
                    return 0;

                return technicianReviews.Average(r => r.Rating);
            }
        }
        public int TatalFinishedOrders
        {
            get
            {
                if (Order == null || !Order.Any())
                    return 0;
                return Order.Count(o => o.Status == OrderStatus.Finished);
            }
        }
        public int TotalReviews
        {
            get
            {
                if (Reviews == null)
                    return 0;

                return Reviews.Count(r => r.AuthorType == ReviewAuthorType.User);
            }
        }
    }
}
