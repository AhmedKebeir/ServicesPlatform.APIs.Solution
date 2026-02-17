using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Tech_Specs
{
    public class TechnicianSpecifications : BaseSpecifications<Technician>
    {
        public TechnicianSpecifications(string userId)
            : base(T => T.UserId == userId)
        {
            Includes.Add(t => t.Departments);
            Includes.Add(t => t.Reviews);
            Includes.Add(t => t.Order);
        }
        public TechnicianSpecifications(int id)
            : base(T => T.Id == id)
        {
            Includes.Add(t => t.User);
            Includes.Add(t => t.User.Addresses);
            Includes.Add(t => t.Reviews);
            Includes.Add(t => t.Order);
            Includes.Add(t => t.Departments!);
            AddThenInclude(t => t.Departments, td => td.Department);
            

        }
        public TechnicianSpecifications()
            : base()
        {
            Includes.Add(t => t.User);
        }
        public TechnicianSpecifications(bool isActive)
            : base(T => T.IsActive == isActive)
        {

        }

        public TechnicianSpecifications(TechniciansSpecParams specParams)
            : base(t =>
            (string.IsNullOrEmpty(specParams.Search) || t.User.DisplayName.ToLower().Contains(specParams.Search)) &&

            (string.IsNullOrEmpty(specParams.City)
                 || t.User.Addresses.Any(a => a.City.ToLower().Contains(specParams.City.ToLower()))) &&

            (string.IsNullOrEmpty(specParams.Center)
                     || t.User.Addresses.Any(a => a.Center.ToLower().Contains(specParams.Center.ToLower())))&&

            (!specParams.IsActive.HasValue || t.IsActive == specParams.IsActive) &&

            (!specParams.DepartmentId.HasValue || t.Departments.Any(d => d.DepartmentId == specParams.DepartmentId))




            )
        {
            Includes.Add(t => t.User);
            Includes.Add(t => t.User.Addresses);
            Includes.Add(t => t.Reviews);
            Includes.Add(t => t.Departments!);
            AddThenInclude(t => t.Departments, td => td.Department);




            // Sorting
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    // أعلى تقييم فقط
                    case "ratingDesc":
                        AddOrderByDesc(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));
                        break;

                    // أقل تقييم فقط
                    case "ratingAsc":
                        AddOrderBy(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));
                        break;

                    // 🔥 أعلى تقييم + أكبر عدد مراجعات
                    case "ratingTop":
                        AddOrderByDesc(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));

                        AddOrderByDesc(t => t.Reviews.Count);
                        break;

                    // 🔥 أقل تقييم + أقل عدد مراجعات
                    case "ratingTopAsc":
                        AddOrderBy(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));

                        AddOrderBy(t => t.Reviews.Count);
                        break;

                    // خبرة
                    case "experienceAsc":
                        AddOrderBy(t => t.ExperienceYears);
                        break;

                    case "experienceDesc":
                        AddOrderByDesc(t => t.ExperienceYears);
                        break;

                    default:
                        AddOrderByDesc(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));

                        AddOrderBy(t => t.Reviews.Count);
                        break;
                }
            }
            else
            {
                AddOrderByDesc(t =>
                            t.Reviews.Count == 0 ? 0 :
                            t.Reviews.Average(r => r.Rating));

                AddOrderByDesc(t => t.Reviews.Count);
                // default
            }


            ApplyPagination((specParams.PageIndex - 1) * specParams.PageSize, specParams.PageSize);

        }
    }
}
