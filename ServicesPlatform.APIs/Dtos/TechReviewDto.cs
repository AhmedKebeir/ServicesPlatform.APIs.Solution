using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class TechReviewDto
    {
        [Required]
        public string Comment { get; set; }
        [Required]
        public int Rating { get; set; }
        public IFormFile? ImageUrl { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int OrderId { get; set; }
    }
}
