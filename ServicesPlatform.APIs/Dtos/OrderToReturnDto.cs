namespace ServicesPlatform.APIs.Dtos
{
    public class OrderToReturnDto
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset ScheduledDate { get; set; }
        public string Status { get; set; }
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> ImageUrls { get; set; }

        public UserToReturnDto User { get; set; }

        public TechnicianUserDto Technician { get; set; }
        public OrderAddressDto Address { get; set; }

        // 👇 الجديد
        public bool IsLocked { get; set; }

    }
}
