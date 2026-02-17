using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class OrderImageUrlsResolver : IValueResolver<Order, OrderToReturnDto, List<string>>
    {
        private readonly IConfiguration configuration;

        public OrderImageUrlsResolver(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public List<string> Resolve(Order source, OrderToReturnDto destination, List<string> destMember, ResolutionContext context)
        {
            if (source.Images is null || !source.Images.Any())
                return new List<string>();

            var baseUrl = configuration["ApiBaseUrl"];

            return source.Images.Select(i => $"{baseUrl}/files/orders/{i.ImageUrl}").ToList();
        }
    }
}
