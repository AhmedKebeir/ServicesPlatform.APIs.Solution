using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class DepartmentToRetuenDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }
}
