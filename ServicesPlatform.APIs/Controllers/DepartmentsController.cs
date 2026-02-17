using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesPlatform.APIs.Dtos;
using ServicesPlatform.APIs.Errors;
using ServicesPlatform.APIs.Helpers;
using ServicesPlatform.Core;
using ServicesPlatform.Core.Entities;
using ServicesPlatform.Core.Specifications.Department_Specs;

namespace ServicesPlatform.APIs.Controllers
{

    public class DepartmentsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<DepartmentToRetuenDto>>> GetAllDepartments()
        {
            var departments = await _unitOfWork.Repository<Department>().GetAllAsync();

            if (departments is null || !departments.Any())
                return NotFound(new ApiResponse(404, "has no any department!"));

            return Ok(_mapper.Map<IReadOnlyList<Department>, IReadOnlyList<DepartmentToRetuenDto>>(departments));
        }

        [HttpGet("top-four")]
        public async Task<ActionResult<Pagination<DepartmentToRetuenDto>>> GetTopFourDepartments([FromQuery] DepartmentSpecParams specParams)
        {
            var spec =new  GetDepartmentsWithPaginationSpecifications(specParams);
            var departments = await _unitOfWork.Repository<Department>().GetAllWithSpecAsync(spec);
            var specCount =new  CountDepartmentSpecifications(specParams);
            var count = await _unitOfWork.Repository<Department>().GetCountAsync(specCount);

            var mapping = _mapper.Map<IReadOnlyList<Department>, IReadOnlyList<DepartmentToRetuenDto>>(departments);
            
            return Ok(new Pagination<DepartmentToRetuenDto>(specParams.PageIndex, specParams.PageSize, mapping, count));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DepartmentToRetuenDto>> CreateDepartment(DepartmentDto department)
        {
            if (CheckDepartmentExists(department.Name).Result.Value)
                return BadRequest(new ApiResponse(400, $".هذاالقسم {department.Name} موجود بالفعل "));

            var mapping = _mapper.Map<DepartmentDto, Department>(department);

            var deptRepo = _unitOfWork.Repository<Department>();

            if(department.ImageUrl != null)
            {
                var imageUrl =  DocumentSettings.UploadFile(department.ImageUrl, "Departments");
                mapping.ImageUrl = imageUrl;
            }

            await deptRepo.AddAsync(mapping);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400));


            return Ok(_mapper.Map<DepartmentToRetuenDto>(mapping));
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteDepartment(int id)
        {
            var deptRepo = _unitOfWork.Repository<Department>();
            var department =  await deptRepo.GetAsync(id);

            if (department is null)
                return NotFound(new ApiResponse(404, "هذا القسم غير موجود ."));

            deptRepo.Delete(department);

            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400));

            return Ok("تم حذق القسم بنجاح .");

        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentToRetuenDto>> GetDepartmentById(int id)
        {
            var department = await _unitOfWork.Repository<Department>().GetAsync(id);
            if (department is null)
                return NotFound(new ApiResponse(404, "هذا القسم غير موجود ."));
            return Ok(_mapper.Map<DepartmentToRetuenDto>(department));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDepartment(int id, DepartmentDto department)
        {
            var deptRepo = _unitOfWork.Repository<Department>();
            var existingDepartment = await deptRepo.GetAsync(id);
            if (existingDepartment is null)
                return NotFound(new ApiResponse(404, "هذا القسم غير موجود ."));
            if (existingDepartment.Name != department.Name)
            {
                if (CheckDepartmentExists(department.Name).Result.Value)
                    return BadRequest(new ApiResponse(400, $".هذاالقسم {department.Name} موجود بالفعل "));
            }
            _mapper.Map(department, existingDepartment);
            if(department.ImageUrl != null)
            {
                // Delete Old Image
                if (!string.IsNullOrEmpty(existingDepartment.ImageUrl))
                {
                    DocumentSettings.DeleteFile(existingDepartment.ImageUrl, "Departments");
                }
                // Upload New Image
                var imageUrl = DocumentSettings.UploadFile(department.ImageUrl, "Departments");
                existingDepartment.ImageUrl = imageUrl;
            }
            deptRepo.Update(existingDepartment);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400));
            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("departmentexists")]
        public async Task<ActionResult<bool>> CheckDepartmentExists(string name)
        {
            var spec = new GetDepartmentByNameSpecifications(name);

            return await _unitOfWork.Repository<Department>().GetWithSpec(spec) is not null;
        }




    
    }
}
