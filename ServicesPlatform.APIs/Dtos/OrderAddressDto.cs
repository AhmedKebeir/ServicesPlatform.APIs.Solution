using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class OrderAddressDto
    {
        [Required]
        public string City { get; set; }
        [Required]
        public string Center { get; set; }
        [Required]
        public string street { get; set; }
    }
}