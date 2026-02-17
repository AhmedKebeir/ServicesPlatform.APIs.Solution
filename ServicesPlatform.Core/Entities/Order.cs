using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesPlatform.Core.Entities
{
    public class Order : BaseEntity
    {
        public Order()
        {

        }
        public Order(string title, string userId, int technicianId, int departmentId, string description, string phoneNumber, OrderAddress address, ICollection<string> imageUrls)
        {
            Title = title;
            UserId = userId;
            TechnicianId = technicianId;
            DepartmentId = departmentId;
            Description = description;
            PhoneNumber = phoneNumber;
            Address = address;
            foreach (var url in imageUrls)
            {
                Images.Add(new Order_Images { ImageUrl = url });
            }
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ScheduledDate { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;//finish
        public string PhoneNumber { get; set; }
        public OrderAddress Address { get; set; }//finish
        public string UserId { get; set; }
        public AppUser User { get; set; }//finish
        public int TechnicianId { get; set; }
        public Technician Technician { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }//finish
        public ICollection<Order_Images> Images { get; set; } = new HashSet<Order_Images>();//finish
        public ICollection<Review>? Review { get; set; }//finish

    }
}
