using AutoMapper;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.Core.Entities;

namespace ServicesPlatform.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<UpdateAddressDto, Address>();

            CreateMap<Department, UserDepartmentDto>();

            CreateMap<Department, DepartmentToRetuenDto>()
                .ForMember(d => d.ImageUrl, O => O.MapFrom<DepartmentImageUrlResolver>());
            CreateMap<DepartmentDto, Department>();

            CreateMap<Technicians_Departments, UserDepartmentDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Department.Id))
                .ForMember(d => d.Name, O => O.MapFrom(s => s.Department.Name));

            CreateMap<Department, UserDepartmentDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                .ForMember(d => d.Name, O => O.MapFrom(s => s.Name));

            CreateMap<AppUser, UserToReturnDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                .ForMember(d => d.FullName, O => O.MapFrom(s => s.FullName))
                .ForMember(d => d.DisplayName, O => O.MapFrom(s => s.DisplayName))
                .ForMember(d => d.Email, O => O.MapFrom(s => s.Email))
                .ForMember(d => d.Image, O => O.MapFrom<UserImageUrlResolver>())
                .ForMember(d => d.Addresses, O => O.MapFrom(s => s.Addresses))
                .ForMember(d => d.PhoneNumber, O => O.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Bio, O => O.MapFrom(s => s.Bio))
                 .ForMember(d => d.IsActive, O => O.MapFrom(s => s.Technician.IsActive))
                 .ForMember(d => d.IsVerified, O => O.MapFrom(s => s.Vefify))
                .ForMember(d => d.TotalReviews, O => O.MapFrom(s => s.TotalReviews))
                .ForMember(d => d.AverageRating, O => O.MapFrom(s => s.AverageRating))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Token, opt => opt.Ignore());
            CreateMap<AppUser, UserDto>()
               .ForMember(d => d.FullName, O => O.MapFrom(s => s.FullName))
               .ForMember(d => d.DisplayName, O => O.MapFrom(s => s.DisplayName))
               .ForMember(d => d.Email, O => O.MapFrom(s => s.Email))
               .ForMember(d => d.Image, O => O.MapFrom<UserImageUrlDtoResolver>())
               .ForMember(d => d.Addresses, O => O.MapFrom(s => s.Addresses));
            ;

            //CreateMap<Technician, UserToReturnDto>()
            //  .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            //  .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            //  .ForMember(dest => dest.Image, opt => opt.MapFrom<TechnicianUserImageUrlResolver>())
            //  .ForMember(d => d.Addresses, O => O.MapFrom(s => s.User.Addresses))
            //  .ForMember(dest => dest.Role, opt => opt.Ignore()) // لو مش محتاجه هنا
            //  .ForMember(dest => dest.Token, opt => opt.Ignore());

            CreateMap<Technician, TechnicianUserDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                .ForMember(d => d.ExperienceYears, O => O.MapFrom(s => s.ExperienceYears))
                .ForMember(d => d.IsActive, O => O.MapFrom(s => s.IsActive))
                .ForMember(d => d.AverageRating, O => O.MapFrom(s => s.AverageRating))
                .ForMember(d => d.TotalFinishedOrders, O => O.MapFrom(s => s.TatalFinishedOrders))
                .ForMember(d => d.TotalReviews, O => O.MapFrom(s => s.TotalReviews))
                .ForMember(d => d.FullName, O => O.MapFrom(s => s.User.FullName))
                .ForMember(d => d.DisplayName, O => O.MapFrom(s => s.User.DisplayName))
                .ForMember(d => d.Bio, O => O.MapFrom(s => s.User.Bio))
                 .ForMember(d => d.Bio, O => O.MapFrom(s => s.User.Bio))
                .ForMember(d => d.Departments, O => O.MapFrom(s => s.Departments))
                .ForMember(d => d.CountOfPoints, O => O.MapFrom(s => s.CountOfPoints))

                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Image, opt => opt.MapFrom<TechnicianUserImageUrlResolver>())
                .ForMember(d => d.Addresses, O => O.MapFrom(s => s.User.Addresses));

            CreateMap<Address, AddressToReturnDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                 .ForMember(d => d.City, O => O.MapFrom(s => s.City))
                .ForMember(d => d.Center, O => O.MapFrom(s => s.Center))
                .ForMember(d => d.Street, O => O.MapFrom(s => s.Street));

            CreateMap<OrderAddressDto, OrderAddress>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                .ForMember(d => d.Title, O => O.MapFrom(s => s.Title))
                .ForMember(d => d.Description, O => O.MapFrom(s => s.Description))
                .ForMember(d => d.CreateAt, O => O.MapFrom(s => s.CreateAt))
                .ForMember(d => d.ScheduledDate, O => O.MapFrom(s => s.ScheduledDate))
                .ForMember(d => d.Status, O => O.MapFrom(s => s.Status))
                .ForMember(d => d.Address, O => O.MapFrom(s => s.Address))
                .ForMember(d => d.PhoneNumber, O => O.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.DepartmentName, O => O.MapFrom(s => s.Department.Name))
                .ForMember(d => d.ImageUrls, O => O.MapFrom<OrderImageUrlsResolver>())
                .ForMember(d => d.User, O => O.MapFrom(s => s.User))
                .ForMember(d => d.Technician, O => O.MapFrom(s => s.Technician));

            CreateMap<ReviewDto, Review>()
                .ForMember(d => d.ImageUrl, O => O.Ignore());

            CreateMap<TechReviewDto, Review>()
                .ForMember(d => d.ImageUrl, O => O.Ignore());

            CreateMap<Review, ReviewRoReturnDto>()
                .ForMember(d => d.Id, O => O.MapFrom(s => s.Id))
                .ForMember(d=>d.AuthorType, O => O.MapFrom(s => s.AuthorType.ToString()))
                .ForMember(d => d.Author, O => O.MapFrom(s=>s.AuthorId))
                .ForMember(d => d.Comment, O => O.MapFrom(s => s.Comment))
                .ForMember(d => d.Rating, O => O.MapFrom(s => s.Rating))
                .ForMember(d => d.OrderTitle, O => O.MapFrom(s => s.Order.Title))
                .ForMember(d => d.Date, O => O.MapFrom(s => s.Date))
                .ForMember(d => d.User, O => O.MapFrom(s => s.User))
                .ForMember(d => d.Technician, O => O.MapFrom(s => s.Technician))
                .ForMember(d => d.ImageUrl, O => O.MapFrom<ReviewImageUrlResolver>());




        }
    }
}
