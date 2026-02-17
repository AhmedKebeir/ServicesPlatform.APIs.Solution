namespace ServicesPlatform.APIs.Dtos
{
    public class ReviewRoReturnDto
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string? ImageUrl { get; set; }
        public string? OrderTitle { get; set; }

        public string AuthorType { get; set; }
        public string Author { get; set; }
        public DateTimeOffset Date { get; set;  }
       public UserDto User { get; set; }
        public TechnicianUserDto Technician { get; set; }
        
        
    }
}
