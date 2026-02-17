using ServicesPlatform.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class ReviewDto
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public int Rating { get; set; }
        public IFormFile? ImageUrl { get; set; }
        [Required]
        public int TechnicianId { get; set; }
        [Required]
        public int OrderId { get; set; }
        
    }
}
