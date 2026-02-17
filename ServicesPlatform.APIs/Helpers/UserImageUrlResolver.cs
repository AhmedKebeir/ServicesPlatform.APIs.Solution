using AutoMapper;
using Microsoft.Extensions.Configuration;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class UserImageUrlResolver : IValueResolver<AppUser, UserToReturnDto, string>
    {
        private readonly IConfiguration _configuration;

        public UserImageUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(AppUser source, UserToReturnDto destination, string destMember, ResolutionContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(source.Image))
                    return string.Empty;

                var baseUrl = _configuration["ApiBaseUrl"];
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
