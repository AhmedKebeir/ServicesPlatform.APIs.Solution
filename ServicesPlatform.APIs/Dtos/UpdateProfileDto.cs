using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class UpdateProfileDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        
        public string? Bio { get; set; }
    }
}
