using ServicesPlatform.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Specifications.Order_Specs
{
    public class CountOrdersWithSpecParams : BaseSpecifications<Order>
    {
        public CountOrdersWithSpecParams(OrderSpecParams specParams)
            : base(o =>
                    (!specParams.TechnicianId.HasValue || o.TechnicianId == specParams.TechnicianId.Value) &&
                    (string.IsNullOrEmpty(specParams.UserId) || o.UserId == specParams.UserId) &&
                    (!specParams.DepartmentId.HasValue || o.DepartmentId == specParams.DepartmentId.Value) &&
                    (!specParams.Date.HasValue || DateOnly.FromDateTime(o.CreateAt.DateTime) == specParams.Date.Value) &&
                    (string.IsNullOrEmpty(specParams.Status.ToString()) || o.Status == specParams.Status))
        {

        }

        public CountOrdersWithSpecParams(OrderStatus orderStatus, int techId)
            : base(o =>
                o.Status == orderStatus && o.TechnicianId == techId
            )
        {

        }
        public CountOrdersWithSpecParams(int techId)
            : base(o =>
           (o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Pending && o.Status != OrderStatus.Finished) &&
            (o.TechnicianId == techId)

            )
        {


        }
        public CountOrdersWithSpecParams(string userId)
            : base(o =>
           (o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Pending && o.Status != OrderStatus.Finished) &&
            (o.UserId == userId)
            )
        {
        }
    }
}
