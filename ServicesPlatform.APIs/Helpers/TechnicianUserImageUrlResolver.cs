using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class TechnicianUserImageUrlResolver : IValueResolver<Technician, TechnicianUserDto, string>
    {
        private readonly IConfiguration configuration;

        public TechnicianUserImageUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Technician source, TechnicianUserDto destination, string destMember, ResolutionContext context)
        {
            if(string.IsNullOrEmpty(source.User.Image))
                return string.Empty;

            return $"{configuration["ApiBaseUrl"]}/files/users/images/{source.User.Image}";
        }
    }
}
