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
using ServicesPlatform.Core.Specifications.TechDept_Specs;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace ServicesPlatform.APIs.Controllers
{

    public class TechnicianController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public TechnicianController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IAuthService authService
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _authService = authService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<UserToReturnDto>> TechnicianRegister([FromForm] TechDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);


            var techSpec = new TechnicianSpecifications(user.Id);
            var existsTechnician = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);

            if (existsTechnician is not null)
                return BadRequest(new ApiResponse(400, "this Technician is already exists"));





            var technician = new Technician
            {

                UserId = user.Id,
                IsActive = true,
                ExperienceYears = model.ExperienceYears,
                IdCard = model.IdCard is not null ? DocumentSettings.UploadFile(model.IdCard, "Cards") : "",

            };

            await _unitOfWork.Repository<Technician>().AddAsync(technician);
            var saveTech = await _unitOfWork.CompleteAsync();

            if (saveTech <= 0)
                return BadRequest(new ApiResponse(400, "Failed to register technician."));

            if (model?.Departments?.Count > 0)
            {
                var departmentRepo = _unitOfWork.Repository<Department>();
                var techDepartmentsRepo = _unitOfWork.Repository<Technicians_Departments>();
                foreach (var item in model.Departments)
                {
                    var department = await departmentRepo.GetAsync(item.Id);
                    if (department is not null)
                    {

                        await techDepartmentsRepo.AddAsync(new Technicians_Departments
                        {
                            TechnicianId = technician.Id,
                            DepartmentId = department.Id,
                        });
                    }

                }

                var result = await _unitOfWork.CompleteAsync();

                if (result <= 0)
                    return BadRequest(new ApiResponse(400, "Failed Add Departments."));

            }




            if (await _userManager.IsInRoleAsync(user, "User"))
                await _userManager.RemoveFromRoleAsync(user, "User");

            await _userManager.AddToRoleAsync(user, "Technician");


            var token = await _authService.CreateTokenAsync(user, _userManager, false);
            var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
            if (setToken.Succeeded is false)
                return Unauthorized(new ApiResponse(401));


            return Ok(new UserToReturnDto
            {
                DisplayName = user.DisplayName,
                FullName = user.FullName,
                Email = user.Email,
                Role = "Technician",
                Token = token,
                Image = user.Image ?? ""
            });
        }

        [Authorize(Roles = "Technician")]
        [HttpGet]
        public async Task<ActionResult<UserAndDepartmentsToReturnDto>> GetTechDepartments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);

            if (tech is null) return NotFound(new ApiResponse(400, "This Technician is not found!"));

            var techDeptSpce = new TechDeptsSpceifications(tech.Id);
            var techDept = await _unitOfWork.Repository<Technicians_Departments>().GetAllWithSpecAsync(techDeptSpce);

            if (techDept is null)
                return NotFound(new ApiResponse(400, "This Technician has no any departments"));

            var mapping = _mapper.Map<List<Department>, List<UserDepartmentDto>>(techDept.Select(u => u.Department).ToList());

            var token = await _userManager.GetAuthenticationTokenAsync(user, "ServicesPlatform", "Token");
            if (token is null)
            {
                token = await _authService.CreateTokenAsync(user, _userManager, false);
                var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
                if (setToken.Succeeded is false)
                    return Unauthorized(new ApiResponse(401));
            }

            var role = await _userManager.GetRolesAsync(user);
            return Ok(new UserAndDepartmentsToReturnDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image ?? "",
                DisplayName = user.DisplayName,
                IsActive = tech.IsActive,
                CountOfPoints= tech.CountOfPoints ?? 0,
                Role = role.FirstOrDefault() ?? "Tech",
                Token = token,
                Departments = mapping

            });


        }

        [Authorize(Roles = "Technician")]
        [HttpPut("update-state/{state}")]
        public async Task<ActionResult<UserAndDepartmentsToReturnDto>> UpdateState(bool state)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var techRepo = _unitOfWork.Repository<Technician>();

            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await techRepo.GetWithSpec(techSpec);

            if (tech is null) return NotFound(new ApiResponse(400, "This Technician is not found!"));

            var techDeptSpce = new TechDeptsSpceifications(tech.Id);
            var techDept = await _unitOfWork.Repository<Technicians_Departments>().GetAllWithSpecAsync(techDeptSpce);

            if (techDept is null)
                return NotFound(new ApiResponse(400, "This Technician has no any departments"));

            var mapping = _mapper.Map<List<Department>, List<UserDepartmentDto>>(techDept.Select(u => u.Department).ToList());

            if (tech.IsActive == state)
                return BadRequest(new ApiResponse(400));

            tech.IsActive = state;

            techRepo.Update(tech);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "please try again!"));

            var token = await _userManager.GetAuthenticationTokenAsync(user, "ServicesPlatform", "Token");
            var role = await _userManager.GetRolesAsync(user);
            return Ok(new UserAndDepartmentsToReturnDto()
            {
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image ?? "",
                IsActive = state,
                Role = role.FirstOrDefault() ?? "Tech",
                Token = token,
                Departments = mapping

            });

        }


        [Authorize]
        [HttpGet("technician-department/{id}")]
        public async Task<ActionResult<IReadOnlyList<TechnicianUserDto>>> GetAllTechniciansByDepartment(int id)
        {
            var spec = new TechniciansByDepartmentSpecifications(id);
            var technicians = await _unitOfWork.Repository<Technician>().GetAllWithSpecAsync(spec);


            if (technicians is null || !technicians.Any())
                return NotFound(new ApiResponse(404, "this department has no technicians"));

            var mapping = _mapper.Map<IReadOnlyList<TechnicianUserDto>>(technicians);

            return Ok(mapping);
        }

        [Authorize]
        [HttpGet("technician-department-area/{deptId}/{city}/{center}")]
        public async Task<ActionResult<IReadOnlyList<TechnicianUserDto>>> GetAllTechniciansByAddress(int deptId, string city, string center)
        {
            var spec = new GetTechniciansBayDepartmentAndAreaSpecifications(deptId, city, center);
            var technicians = await _unitOfWork.Repository<Technician>().GetAllWithSpecAsync(spec);


            if (technicians is null || !technicians.Any())
                return NotFound(new ApiResponse(404, "this department has no technicians"));

            var mapping = _mapper.Map<IReadOnlyList<TechnicianUserDto>>(technicians);

            return Ok(mapping);
        }

        [Authorize(Roles = "Technician")]
        [HttpPut("update-department/{id}")]
        public async Task<ActionResult<string>> UpdateDepartment(int id, UserDepartmentDto department)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var existDept = await _unitOfWork.Repository<Department>().GetAsync(department.Id);
            if (existDept is null) return NotFound(new ApiResponse(401, "department is not found!"));


            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);

            if (tech is null) return NotFound(new ApiResponse(400, "This Technician is not found!"));
            var techDeptRepo = _unitOfWork.Repository<Technicians_Departments>();
            var spec = new TechDeptSpceifications(tech.Id, id);
            var dept = await techDeptRepo.GetWithSpec(spec);

            if (dept is null) return NotFound(new ApiResponse(401, "department is not found!"));

            techDeptRepo.Delete(dept);
            await techDeptRepo.AddAsync(new Technicians_Departments
            {
                TechnicianId = tech.Id,
                DepartmentId = department.Id,
            });



            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed Update Departments."));

            return Ok("Updating Department is successed");
        }

        [Authorize(Roles = "Technician")]
        [HttpDelete("{deptId}")]
        public async Task<ActionResult<string>> DeleteTechDepartment(int deptId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);

            var techDeptSpec = new TechDeptSpceifications(tech.Id, deptId);
            var techDeptRepo = _unitOfWork.Repository<Technicians_Departments>();
            var techDept = await techDeptRepo.GetWithSpec(techDeptSpec);
            if (techDept is null)
                return BadRequest(new ApiResponse(400));

            techDeptRepo.Delete(techDept);

            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed Update Departments."));

            return Ok("Delete Department is successed");

        }

        [Authorize]
        [HttpGet("page-technicians")]
        public async Task<ActionResult<Pagination<TechnicianUserDto>>> GetTechniciansByPagination([FromQuery] TechniciansSpecParams specParams)
        {
            var techRepo = _unitOfWork.Repository<Technician>();

            var spec = new TechnicianSpecifications(specParams);
            var technincians = await techRepo.GetAllWithSpecAsync(spec);

            var mapping = _mapper.Map<IReadOnlyList<Technician>, IReadOnlyList<TechnicianUserDto>>(technincians);

            var countSpec = new CountTechnicianPaginationSepcifications(specParams);
            var count = await techRepo.GetCountAsync(countSpec);





            return Ok(new Pagination<TechnicianUserDto>(specParams.PageIndex, specParams.PageSize, mapping, count));


        }

        [Authorize(Roles = "Technician")]
        [HttpGet("technician-dashboard")]
        public async Task<ActionResult<TechnicianDashboard>> GetTechnicianDashboard()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var technicianSpec = new TechnicianSpecifications(user.Id);
            var technician = await _unitOfWork.Repository<Technician>().GetWithSpec(technicianSpec);

            if (technician is null)
                return NotFound(new ApiResponse(404, "Technician not found"));

            var orderReop = _unitOfWork.Repository<Order>();

            var techOrderPendingSpec = new CountOrdersWithSpecParams(OrderStatus.Pending, technician.Id);
            var totalNewOrders = await orderReop.GetCountAsync(techOrderPendingSpec);

            var totalActiveOrdersSpec = new CountOrdersWithSpecParams(technician.Id);
            var totalActiveOrders = await orderReop.GetCountAsync(totalActiveOrdersSpec);

            var techOrderFinishedSpec = new CountOrdersWithSpecParams(OrderStatus.Finished, technician.Id);
            var totalCompletedOrders = await orderReop.GetCountAsync(techOrderFinishedSpec);


            var CountOfPoints = technician.CountOfPoints ?? 0;


            var ordersParams = new OrderSpecParams();
            ordersParams.TechnicianId = technician.Id;
            ordersParams.PageIndex = 1;
            ordersParams.PageSize = 2;
            ordersParams.Status = OrderStatus.Pending;

            var pendingOrdersSpec = new GetAllOrdersSpecifications(ordersParams);
            var pendingOrders = await orderReop.GetAllWithSpecAsync(pendingOrdersSpec);


            var mappingOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(pendingOrders);


            var activeOrdersParams = new OrderSpecParams();
            activeOrdersParams.TechnicianId = technician.Id;
            activeOrdersParams.PageIndex = 1;
            activeOrdersParams.PageSize = 2;

            var activeOrdersSpec = new GetAllActiveOrdersSpecifications(activeOrdersParams);
            var activeOrders = await orderReop.GetAllWithSpecAsync(activeOrdersSpec);

            var mappingActiveOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(activeOrders);

            // ================= Lock Orders if no points =================
            if ((technician.CountOfPoints ?? 0) <= 0)
            {
                void LockOrders(IEnumerable<OrderToReturnDto> orders)
                {
                    foreach (var order in orders)
                    {
                        order.IsLocked = true;
                        order.Title = "🔒 طلب مقفول";
                        order.Description = "اشحن نقاطك لعرض تفاصيل الطلب";
                        order.PhoneNumber = "********";
                        order.ImageUrls = new List<string>();
                        order.User = new UserToReturnDto { FullName = "🔒 مخفي",DisplayName= "🔒 مخفي" };
                        order.Address = new OrderAddressDto
                        {
                            City = "🔒 مخفي",
                            Center = "🔒 مخفي",
                            street = "🔒 مخفي"
                        };
                    }
                }

                LockOrders(mappingOrders);
                
            }

            var dashboard = new TechnicianDashboard
            {
                TotalNewOrders = totalNewOrders,
                TotalActiveOrders = totalActiveOrders,
                TotalCompletedOrders = totalCompletedOrders,
                AverageRating = technician.AverageRating,
                NewOrders = mappingOrders,
                CountOfPoints = CountOfPoints,
                ActiveOrders = mappingActiveOrders
            };

            return Ok(dashboard);
        }

        [Authorize]
        [HttpGet("top-technicians")]
        public async Task<ActionResult<Pagination<TechnicianUserDto>>> GetTopTechnincains([FromQuery] TechniciansSpecParams specParams)
        {
            var techRepo = _unitOfWork.Repository<Technician>();
            var spec = new TechnicianSpecifications(specParams);
            var technincians = await techRepo.GetAllWithSpecAsync(spec);
            var mapping = _mapper.Map<IReadOnlyList<Technician>, IReadOnlyList<TechnicianUserDto>>(technincians);
            var countSpec = new CountTechnicianPaginationSepcifications(specParams);
            var count = await techRepo.GetCountAsync(countSpec);
            return Ok(new Pagination<TechnicianUserDto>(specParams.PageIndex, specParams.PageSize, mapping, count));


        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<TechnicianUserDto>> GetTechnicianById(int id)
        {
            var techSpec = new TechnicianSpecifications(id);
            var technician = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);
            if (technician is null)
                return NotFound(new ApiResponse(404, "Technician not found"));
            var mapping = _mapper.Map<TechnicianUserDto>(technician);
            return Ok(mapping);
        }




        [Authorize]
        [HttpGet("available-departments")]
        public async Task<ActionResult<IReadOnlyList<DepartmentToRetuenDto>>> GetAvailableDepartmentsForTech()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);
            if (tech is null) return NotFound(new ApiResponse(400, "This Technician is not found!"));
            var techDeptSpce = new TechDeptsSpceifications(tech.Id);
            var techDept = await _unitOfWork.Repository<Technicians_Departments>().GetAllWithSpecAsync(techDeptSpce);
            var assignedDeptIds = techDept.Select(td => td.DepartmentId).ToList();
            var allDepartments = await _unitOfWork.Repository<Department>().GetAllAsync();
            var availableDepartments = allDepartments
                .Where(dept => !assignedDeptIds.Contains(dept.Id))
                .ToList();
            if (availableDepartments is null || !availableDepartments.Any())
                return NotFound(new ApiResponse(404, "No available departments for this technician."));
            var mapping = _mapper.Map<IReadOnlyList<Department>, IReadOnlyList<DepartmentToRetuenDto>>(availableDepartments);
            return Ok(mapping);

        }

        [Authorize]
        [HttpPost("add-depatments")]
        public async Task<ActionResult<string>> AddDepartmentsToTechnician([FromBody] List<UserDepartmentDto> departments)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var techSpec = new TechnicianSpecifications(user.Id);
            var tech = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);
            if (tech is null) return NotFound(new ApiResponse(400, "This Technician is not found!"));
            var departmentRepo = _unitOfWork.Repository<Department>();
            var techDepartmentsRepo = _unitOfWork.Repository<Technicians_Departments>();
            foreach (var item in departments)
            {
                var department = await departmentRepo.GetAsync(item.Id);
                if (department is not null)
                {
                    var techDeptSpec = new TechDeptSpceifications(tech.Id, department.Id);
                    var existingTechDept = await techDepartmentsRepo.GetWithSpec(techDeptSpec);
                    if (existingTechDept is null)
                    {
                        await techDepartmentsRepo.AddAsync(new Technicians_Departments
                        {
                            TechnicianId = tech.Id,
                            DepartmentId = department.Id,
                        });
                    }
                }
            }
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to add Departments."));
            return Ok("Departments added successfully.");
        }
    }
}
