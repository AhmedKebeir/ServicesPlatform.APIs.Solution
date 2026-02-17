using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.APIs.Errors;
using ServicesPlatform.APIs.Helpers;
using ServicesPlatform.Core;

using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Services.Contract;
using ServicesPlatform.Core.Specifications.Order_Specs;
using ServicesPlatform.Core.Specifications.Tech_Specs;
using ServicesPlatform.Core.Specifications.Users_Specs;
using ServicesPlatform.Repositories.Data;
using System.Security.Claims;




namespace ServicesPlatform.APIs.Controllers
{

    public class AdminDashboardController : BaseApiController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly AppIdentityDbContext _dbContext;

        public AdminDashboardController(IUnitOfWork unitOfWork,
            UserManager<AppUser> userManager,
            IMapper mapper,
            AppIdentityDbContext dbContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _dbContext = dbContext;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("overview")]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
        {
            // 1. get Total users
            var totalUsers = (await _userManager.GetUsersInRoleAsync("User")).Count;


            //2. get total technician 
            var techRepo = _unitOfWork.Repository<Technician>();
            var totalTechnicians = (await techRepo.GetAllAsync()).Count;

            //3. get total technicinas is active
            var techActiveSpec = new TechnicianSpecifications(true);
            var totalActiveTechnicians = await _unitOfWork.Repository<Technician>().GetCountAsync(techActiveSpec);

            //4. get all orders
            var totalOrders = (await _unitOfWork.Repository<Order>().GetAllAsync()).Count;

            //5. last orders 
            var orderSpecParams = new OrderSpecParams
            {
                PageIndex = 1,
                PageSize = 5,
            };
            var orderSpec = new GetAllOrdersSpecifications(orderSpecParams);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(orderSpec);


            var orderMapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);


            //6. best technicians
            var techSpecParams =new  TechniciansSpecParams
            {
                Sort = "ratingTop",
                PageIndex = 1,
                PageSize = 5,
            };
            var techSpec = new TechnicianSpecifications(techSpecParams);
            var topTechnicians = await techRepo.GetAllWithSpecAsync(techSpec);
            var techMapping = _mapper.Map<IReadOnlyList<TechnicianUserDto>>(topTechnicians);

            return Ok(new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalTechnicians = totalTechnicians,
                TotalsActiveTechnicians = totalActiveTechnicians,
                OrderCounter = totalOrders,
                LastOrders = orderMapping,
                TopTechnicians = techMapping,
            });

        }



        [ProducesResponseType(typeof(Pagination<UserToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Admin")]
        [HttpGet("all-users")]
        public async Task<ActionResult<Pagination<UserToReturnDto>>> GetAllUsers([FromQuery] UsersSpecParams specParams)
        {
            var query = _userManager.Users
                        .Include(u => u.Addresses) // استرجاع العناوين
                        .AsQueryable();

            // فلترة حسب Role داخل DB
            query = from u in query
                    join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                    join r in _dbContext.Roles on ur.RoleId equals r.Id
                    where r.Name == "User"
                    select u;


            // Filtering
            if (!string.IsNullOrEmpty(specParams.Search))
                query = query.Where(u => u.DisplayName.ToLower().Contains(specParams.Search));

            if (!string.IsNullOrEmpty(specParams.City))
                query = query.Where(u => u.Addresses.Any(a => a.City.ToLower() == specParams.City.ToLower()));

            if (!string.IsNullOrEmpty(specParams.Center))
                query = query.Where(u => u.Addresses.Any(a => a.Center.ToLower() == specParams.Center.ToLower()));


            // Sorting (OrderBy افتراضي لتجنب التحذير)
            query = specParams.Sort switch
            {
                "nameAsc" => query.OrderBy(u => u.DisplayName),
                "nameDesc" => query.OrderByDescending(u => u.DisplayName),
                _ => query.OrderBy(u => u.Id) // OrderBy افتراضي
            };


            var count = await query.CountAsync();

            var users = await query
                 .Skip((specParams.PageIndex - 1) * specParams.PageSize)
                 .Take(specParams.PageSize)
                  .ToListAsync();


            var mapping = _mapper.Map<IReadOnlyList<AppUser>, IReadOnlyList<UserToReturnDto>>(users);

            return Ok(new Pagination<UserToReturnDto>(specParams.PageIndex, specParams.PageSize, mapping, count));

        }


        [Authorize(Roles = "Admin")]
        [HttpGet("order/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderDetails(int id)
        {


            var order = await _unitOfWork.Repository<Order>().GetAsync(id);
            if (order is null)
                return NotFound(new ApiResponse(404, "order not found"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(mapping);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("technician/points/{techId}")]
        public async Task<ActionResult> UpdateTechnicianPoints(int techId, [FromBody] UpdatePointsDto dto)
        {
            var techRepo = _unitOfWork.Repository<Technician>();
            var technician = await techRepo.GetAsync(techId);
            if (technician is null)
                return NotFound(new ApiResponse(404, "Technician not found"));

            if (dto.Points < 0)
                return BadRequest(new ApiResponse(400, "Points cannot be negative"));

           if(technician.CountOfPoints is null)
                technician.CountOfPoints = 0;

            technician.CountOfPoints += dto.Points;
            techRepo.Update(technician);

             var result  = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to update technician points"));

            return Ok();

        }
    }
}
