namespace ServicesPlatform.APIs.Dtos
{
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Image { get; set; }
        public List<AddressToReturnDto> Addresses { get; set; }
    }
}
