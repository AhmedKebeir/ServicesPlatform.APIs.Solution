using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
using ServicesPlatform.Core.Specifications.Review_Specs;
using ServicesPlatform.Core.Specifications.Tech_Specs;
using System.Security.Claims;

namespace ServicesPlatform.APIs.Controllers
{

    public class OrderController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork, IMapper mapper,
            IOrderService orderService
            )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pagin-orders")]
        public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrdersWithPagination([FromQuery] OrderSpecParams specParams)
        {
            var orderRepo = _unitOfWork.Repository<Order>();
            var orderSpec = new GetAllOrdersSpecifications(specParams);
            var orders = await orderRepo.GetAllWithSpecAsync(orderSpec);

            var countspec = new CountOrdersWithSpecParams(specParams);
            var count = await orderRepo.GetCountAsync(countspec);

            var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

            return Ok(new Pagination<OrderToReturnDto>(specParams.PageIndex, specParams.PageSize, mapping, count));

        }


        [Authorize(Roles ="Technician")]
        [HttpGet("orders-by-technician")]
        public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrdersByTechnicianWithPagination([FromQuery] OrderSpecParams specParams)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user?.Technician is null)
                return BadRequest(new ApiResponse(400, "can no get orders"));

            specParams.TechnicianId = user.Technician.Id;
            var orderRepo = _unitOfWork.Repository<Order>();

            int count;
            IReadOnlyList<Order> orders;
            if (specParams.Activity == "active")
            {

                var activeOrderSpec = new GetAllActiveOrdersSpecifications(specParams);
                orders = await orderRepo.GetAllWithSpecAsync(activeOrderSpec);
                var activeCountSpec = new CountOrdersWithSpecParams(user.Technician.Id);
                count = await orderRepo.GetCountAsync(activeCountSpec);

            }
            else
            {
                var orderSpec = new GetAllOrdersSpecifications(specParams);
                orders = await orderRepo.GetAllWithSpecAsync(orderSpec);
                var countspec = new CountOrdersWithSpecParams(specParams);
                count = await orderRepo.GetCountAsync(countspec);
            }



            // 3️⃣ Mapping
            var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);


            // 4️⃣ Lock الطلبات لو النقاط = 0
            if ((user.Technician.CountOfPoints ?? 0) <= 0)
            {
                foreach (var order in mapping)
                {
                    order.IsLocked = true;
                    order.Title = "🔒 طلب مقفول";
                    order.Description = "اشحن نقاطك لعرض تفاصيل الطلب";
                    order.PhoneNumber = "********";
                    order.ImageUrls = new List<string>();
                    order.User = new UserToReturnDto { FullName = "🔒 مخفي", DisplayName = "🔒 مخفي" };
                    order.Address = new OrderAddressDto
                    {
                        City = "🔒 مخفي",
                        Center = "🔒 مخفي",
                        street = "🔒 مخفي"
                    }; ;
                }
            }
            return Ok(new Pagination<OrderToReturnDto>(specParams.PageIndex, specParams.PageSize, mapping, count));

        }

        [Authorize]
        [HttpGet("orders-by-user")]
        public async Task<ActionResult<Pagination<OrderToReturnDto>>> GetOrdersByUserWithPagination([FromQuery] OrderSpecParams specParams)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            specParams.UserId = user.Id;
            var orderRepo = _unitOfWork.Repository<Order>();
            if (specParams.Activity == "active")
            {
                var orderSpec = new GetAllActiveOrdersSpecifications(specParams);
                var orders = await orderRepo.GetAllWithSpecAsync(orderSpec);
                var countspec = new CountOrdersWithSpecParams(user.Id);
                var count = await orderRepo.GetCountAsync(countspec);
                var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
                return Ok(new Pagination<OrderToReturnDto>(specParams.PageIndex, specParams.PageSize, mapping, count));
            }
            else
            {
                var orderSpec = new GetAllOrdersSpecifications(specParams);
                var orders = await orderRepo.GetAllWithSpecAsync(orderSpec);
                var countspec = new CountOrdersWithSpecParams(specParams);
                var count = await orderRepo.GetCountAsync(countspec);
                var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);
                return Ok(new Pagination<OrderToReturnDto>(specParams.PageIndex, specParams.PageSize, mapping, count));
            }


        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var spec = new TechnicianSpecifications(model.TechnicianId);
            var technician = await _unitOfWork.Repository<Technician>().GetWithSpec(spec);

            if (technician is null || technician.UserId == user.Id)
                return BadRequest(new ApiResponse(400, "can no do order"));

            if (CheckOrderExists(user.Id, model.DepartmentId, model.TechnicianId).Result.Value)
                return BadRequest(new ApiResponse(400, "order already exists for this technician and department"));

            if (!technician.Departments.Any(d => d.DepartmentId == model.DepartmentId))
                return BadRequest(new ApiResponse(400, "the technician has not include this department"));

            var address = _mapper.Map<OrderAddressDto, OrderAddress>(model.Address);

            List<string> imageUrls = new List<string>();
            foreach (var image in model.OrderImages)
            {
                var imageUrl = DocumentSettings.UploadFile(image, "Orders");
                imageUrls.Add(imageUrl);
            }
            var order = await _orderService.CreateOrderAsync(model.Title, user.Id, technician.Id, model.DepartmentId, model.Description, model.PhoneNumber, address, imageUrls);

            if (order is null)
                return BadRequest(new ApiResponse(400, "problem in create order"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(mapping);

        }
        [Authorize(Roles = "Technician")]
        [HttpGet("technician")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersByTechnicianId()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var techSpec = new TechnicianSpecifications(user.Id);
            var technician = await _unitOfWork.Repository<Technician>().GetWithSpec(techSpec);
            if (technician is null || technician.UserId != user.Id)
                return BadRequest(new ApiResponse(400, "can no get orders"));

            var spec = new GetAllOrderByTechnicianIdSpecifications(technician.Id);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);


            return Ok(mapping);

        }
        [Authorize]
        [HttpGet("myorders")]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetMyOrders()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var spec = new GetAllOrderByTechnicianIdSpecifications(user.Id);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            var mapping = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

            return Ok(mapping);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
                return Unauthorized(new ApiResponse(401, "غير مصرح لك"));



            var orderSpec = new GetOrderByIdSpecifications(id);
            var order = await _unitOfWork.Repository<Order>().GetWithSpec(orderSpec);

            if (order is null)
                return NotFound(new ApiResponse(404, "الطلب غير موجود"));

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // التحقق من صلاحية الوصول
            if (!isAdmin && order.UserId != user.Id && order.TechnicianId != user.Technician?.Id)
                return Unauthorized(new ApiResponse(401, "ليس لديك صلاحية لمشاهدة هذا الطلب"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);

            // ================= Lock الطلب إذا النقاط = 0 && Pending =================
            var hasNoPoints = (user.Technician?.CountOfPoints ?? null) <= 0;
            if (hasNoPoints && mapping.Status == OrderStatus.Pending.ToString())
            {
                mapping.IsLocked = true;
                mapping.Title = "🔒 طلب مقفول";
                mapping.Description = "اشحن نقاطك لعرض تفاصيل الطلب";
                mapping.PhoneNumber = "********";
                mapping.ImageUrls = new List<string>();
                mapping.User = new UserToReturnDto { FullName = "🔒 مخفي", DisplayName = "🔒 مخفي" };
                mapping.Address = new OrderAddressDto
                {
                    City = "🔒 مخفي",
                    Center = "🔒 مخفي",
                    street = "🔒 مخفي"
                };
            }

            return Ok(mapping);
        }

        [Authorize(Roles = "Technician")]
        [HttpPut("accept/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> AcceptOrder(int id, [FromBody] AcceptOrderDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Technician.CountOfPoints is null || user?.Technician.CountOfPoints <= 0)
                return BadRequest(new ApiResponse(400, "لا يمكنك قبول الطلب، يرجى شحن النقاط أولًا!"));

            var currentDateTime = DateTimeOffset.UtcNow;
            if (dto.ScheduledDate <= currentDateTime)
                return BadRequest(new ApiResponse(400, "تاريخ التنفيذ يجب أن يكون في وقت لاحق"));

            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || user?.Technician?.Id != order.TechnicianId)
                return NotFound(new ApiResponse(404, ""));



            if (order.Status != OrderStatus.Pending)
                return BadRequest(new ApiResponse(400, "لا يمكنك قبول هذا الطلب"));

            order.Status = OrderStatus.Accepted;
            order.ScheduledDate = dto.ScheduledDate;

            user.Technician.CountOfPoints -= 1;

            orderRepo.Update(order);
            _unitOfWork.Repository<Technician>().Update(user.Technician);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in accept order"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(mapping);


        }
        [Authorize(Roles = "Technician")]
        [HttpPut("cancel/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> CancelOrder(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || user?.Technician?.Id != order.TechnicianId)
                return NotFound(new ApiResponse(404, "order not found"));

            if (order.Status == OrderStatus.InProgress || order.Status == OrderStatus.Finished || order.Status == OrderStatus.OnTheWay || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Accepted)
                return BadRequest(new ApiResponse(400, "can not cancel this order"));

            order.Status = OrderStatus.Cancelled;

            orderRepo.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in cancel order"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);

            return Ok(mapping);


        }
        [Authorize(Roles = "Technician")]
        [HttpPut("finish/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> FinishOrder(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || user?.Technician?.Id != order.TechnicianId)
                return NotFound(new ApiResponse(404, "order not found"));

            if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Finished || order.Status == OrderStatus.Pending)
                return BadRequest(new ApiResponse(400, "can not finish this order"));

            order.Status = OrderStatus.Finished;
            orderRepo.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in finish order"));

            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(mapping);
        }

        [Authorize(Roles = "Technician")]
        [HttpPut("InProgress/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> InProgressOrder(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || user?.Technician?.Id != order.TechnicianId)
                return NotFound(new ApiResponse(404, "order not found"));
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.InProgress)
                return BadRequest(new ApiResponse(400, "can not set in progress this order"));
            order.Status = OrderStatus.InProgress;
            orderRepo.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in set in progress order"));
            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(mapping);
        }
        [Authorize(Roles = "Technician")]
        [HttpPut("OnTheWay/{id}")]
        public async Task<ActionResult<OrderToReturnDto>> OnTheWayOrder(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || user?.Technician?.Id != order.TechnicianId)
                return NotFound(new ApiResponse(404, "order not found"));
            if (order.Status == OrderStatus.Pending || order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.OnTheWay || order.Status == OrderStatus.Finished)
                return BadRequest(new ApiResponse(400, "can not set in progress this order"));
            order.Status = OrderStatus.OnTheWay;
            orderRepo.Update(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in set in progress order"));
            var mapping = _mapper.Map<Order, OrderToReturnDto>(order);
            return Ok(mapping);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var orderRepo = _unitOfWork.Repository<Order>();
            var order = await orderRepo.GetAsync(id);
            if (order is null || order.UserId != user.Id)
                return NotFound(new ApiResponse(404, "order not found"));
            if (order.Status != OrderStatus.Pending)
                return BadRequest(new ApiResponse(400, "can not delete this order"));
            orderRepo.Delete(order);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "problem in delete order"));
            return Ok();
        }


        [HttpGet("chech-order-exists/{userId}/{deptId}/{techId}")]
        public async Task<ActionResult<bool>> CheckOrderExists(string userId, int deptId, int techId)
        {
            var orderSpec = new GetOrderByUserIdAndDepartIdAndTechIdSpecifications(userId, deptId, techId);
            var existingReview = await _unitOfWork.Repository<Order>().GetWithSpec(orderSpec);
            return Ok(existingReview is not null);
        }

        [Authorize]
        [HttpGet("get-all-count-orders-user/{email}")]
        public async Task<ActionResult> GetAllCountOrdersByUserEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound(new ApiResponse(404, "المستخدم غير موجود!"));
            var specParamsFinished = new OrderSpecParams
            {
                UserId = user.Id,
                Status =OrderStatus.Finished
               
            };
            var specParamsPending= new OrderSpecParams
            {
                UserId = user.Id,
                Status = OrderStatus.Pending

            };
            var specParamsAll = new OrderSpecParams
            {
                UserId = user.Id,
                

            };

            var countspecFinished = new CountOrdersWithSpecParams(specParamsFinished);
            var countOfFinished = await _unitOfWork.Repository<Order>().GetCountAsync(countspecFinished);

            var countspecPending = new CountOrdersWithSpecParams(specParamsPending);
            var countOfPending = await _unitOfWork.Repository<Order>().GetCountAsync(countspecPending);
            var countspecAll = new CountOrdersWithSpecParams(specParamsAll);
            var countOfAll = await _unitOfWork.Repository<Order>().GetCountAsync(countspecAll);
            return Ok(new
            {
                Finished = countOfFinished,
                Pending = countOfPending,
                All = countOfAll
            });



        }
    }
}
