using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class AddressToReturnDto
    {
        public int Id { get; set; }
        public string City { get; set; }

        public string Center { get; set; }

        public string Street { get; set; }
    }
}
