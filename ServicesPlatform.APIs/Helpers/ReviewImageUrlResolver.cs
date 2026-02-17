using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class ReviewImageUrlResolver : IValueResolver<Review, ReviewRoReturnDto, string>
    {
        private readonly IConfiguration configuration;

        public ReviewImageUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Review source, ReviewRoReturnDto destination, string destMember, ResolutionContext context)
        {
            try
            {
                if (string.IsNullOrEmpty(source.ImageUrl))
                    return string.Empty;

                var baseUrl = configuration["ApiBaseUrl"];
                //if (string.IsNullOrEmpty(baseUrl))
                //    baseUrl = "https://localhost:7186"; // fallback لو مش موجود في config

                return $"{baseUrl}/files/Reviews/{source.ImageUrl}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resolving image URL: {ex.Message}");
            }
        }
    }
}
