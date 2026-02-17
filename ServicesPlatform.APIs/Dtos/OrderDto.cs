using ServicesPlatform.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class OrderDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public List<IFormFile> OrderImages { get; set; }
        public OrderAddressDto Address { get; set; }//finish

    }
}
