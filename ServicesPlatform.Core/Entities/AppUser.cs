using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? Image { get; set; }
        public string? Bio { get; set; }

        public string? OTPCode { get; set; }
        public bool Vefify { get; set; }


        public double AverageRating
        {
            get
            {
                if (Reviews == null || !Reviews.Any())
                    return 0;

                var technicianReviews = Reviews
                    .Where(r => r.AuthorType == ReviewAuthorType.Technician);

                if (!technicianReviews.Any())
                    return 0;

                return technicianReviews.Average(r => r.Rating);
            }
        }

        public int TotalReviews
        {
            get
            {
                if (Reviews == null)
                    return 0;

                return Reviews.Count(r => r.AuthorType == ReviewAuthorType.Technician);
            }
        }

        public Technician? Technician { get; set; }//finish

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();//finish
        public ICollection<Address> Addresses { get; set; } = new HashSet<Address>();//finish
        public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();//finish
    }
}
