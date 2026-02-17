using System.ComponentModel.DataAnnotations;

namespace ServicesPlatform.APIs.Dtos
{
    public class TechDto
    {
        //[Required]
        //public bool IsActive { get; set; }
        [Required]
        public int ExperienceYears { get; set; }
        public IFormFile? IdCard {  get; set; }
        public List<UserDepartmentDto> Departments { get; set; }
    }
}
