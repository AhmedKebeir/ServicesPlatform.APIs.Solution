namespace ServicesPlatform.APIs.Dtos
{
    public class UserAndDepartmentsToReturnDto
    {
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }
        public int CountOfPoints { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public List<UserDepartmentDto> Departments { get; set; } 
    }
}
