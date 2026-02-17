using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class UpdateAddressDto
    {
        
        [Required]
        public string City { get; set; }
        [Required]
        public string Center { get; set; }
        [Required]
        public string Street { get; set; }
    }
}
