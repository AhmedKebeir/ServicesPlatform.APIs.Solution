namespace ServicesPlatform.APIs.Dtos
{
    public class UserToReturnDto
    {
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public string? Image {  get; set; }
        public bool? IsActive { get; set; }
        public bool? IsVerified { get; set; }
        public string PhoneNumber { get; set; }
        public string Bio { get; set; }

        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public List<AddressToReturnDto> Addresses { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }

    }
}
