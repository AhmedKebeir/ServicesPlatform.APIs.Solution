using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class UserImageUrlDtoResolver : IValueResolver<AppUser, UserDto, string>
    {
        private readonly IConfiguration configuration;

        public UserImageUrlDtoResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(AppUser source, UserDto destination, string destMember, ResolutionContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(source.Image))
                    return string.Empty;

                var baseUrl = configuration["ApiBaseUrl"];
                //if (string.IsNullOrEmpty(baseUrl))
                //    baseUrl = "https://localhost:7186"; // fallback لو مش موجود في config

                return $"{baseUrl}/files/users/images/{source.Image}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resolving image URL: {ex.Message}");
            }
        }
    }
}
