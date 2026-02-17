using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class DepartmentImageUrlResolver : IValueResolver<Department, DepartmentToRetuenDto, string>
    {
        private readonly IConfiguration configuration;

        public DepartmentImageUrlResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string Resolve(Department source, DepartmentToRetuenDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.ImageUrl))
                return string.Empty;

            return $"{configuration["ApiBaseUrl"]}/files/departments/{source.ImageUrl}";
        }
    }
}
