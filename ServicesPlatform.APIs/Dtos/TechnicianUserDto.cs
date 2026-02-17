namespace ServicesPlatform.APIs.Dtos
{
    public class TechnicianUserDto
    {
        public int Id { get; set; }
        public int ExperienceYears { get; set; }
        public bool IsActive { get; set; }
        public string FullName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }
        public double? AverageRating { get; set; }
        public int TotalFinishedOrders { get; set; }
        public int TotalReviews { get; set; }
        public int? CountOfPoints { get; set; }
        public List<UserDepartmentDto> Departments { get; set; }
        public List<AddressToReturnDto> Addresses { get; set; }
    }
}
