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
using ServicesPlatform.Core.Specifications.Address_Spec;
using ServicesPlatform.Core.Specifications.Users_Specs;
using ServicesPlatform.Repositories.Data;
using System.Data;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace ServicesPlatform.APIs.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _role;
        private readonly IAuthService _authService;
        private readonly IVerificationCode _verificationCode;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppIdentityDbContext _dbContext;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> role,
            IAuthService authService,
            IVerificationCode verificationCode,
            IMapper mapper,
            IUnitOfWork unitOfWork
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _role = role;
            _authService = authService;
            _verificationCode = verificationCode;
            _mapper = mapper;
            _unitOfWork = unitOfWork;

        }


        [ProducesResponseType(typeof(UserToReturnDto), StatusCodes.Status200OK)]
        [HttpPost("register")]
        public async Task<ActionResult<UserToReturnDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "هذا الحساب موجود بالفعل!" } });

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new ApiResponse(400));

            var resetCode = new Random().Next(1000, 9999).ToString();

            var displayName = model.FirstName + " " + model.LastName;
            var user = new AppUser()
            {
                DisplayName = displayName,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.Phone,
                UserName = model.Email.Split("@")[0],
                OTPCode = resetCode,
                Vefify = false
            };



            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded is false) return BadRequest(new ApiResponse(400));

            await _userManager.AddToRoleAsync(user, "User");
            var sendVerification = await _verificationCode.SendVerificationCode(user.Email, "OTP Code", $"Your verification code is: {resetCode}");


            var token = await _authService.CreateTokenAsync(user, _userManager, false);
            var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
            if (setToken.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);
            mapping.Role = "User";
            mapping.Token = token;

            return Ok(mapping);


        }

        [ProducesResponseType(typeof(UserToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<ActionResult<UserToReturnDto>> Login(LoginDto model)
        {
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded is false)
                return Unauthorized(new ApiResponse(401));




            var token = await _authService.CreateTokenAsync(user, _userManager, false);
            var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
            if (setToken.Succeeded is false)
                return Unauthorized(new ApiResponse(401));


            var role = await _userManager.GetRolesAsync(user);

            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);

            mapping.Role = role.FirstOrDefault() ?? "";
            mapping.Token = token;


            return Ok(mapping);

        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return Unauthorized(new ApiResponse(401));

            await _userManager.RemoveAuthenticationTokenAsync(
                user,
                "ServicesPlatform",
                "Token"
            );

            return Ok(new { message = "Logged out successfully" });
        }


        [HttpPost("forget-password")]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return BadRequest(new ApiResponse(400, "user is not found!"));
            var resetCode = new Random().Next(1000, 9999).ToString();
            var sendVerification = await _verificationCode.SendVerificationCode(user.Email, "OTP Code For Reset Passsword", $"Your verification code is: {resetCode}");
            if (!sendVerification) return BadRequest(new ApiResponse(400, "code has not send!"));
            user.OTPCode = resetCode;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded is false) return BadRequest(new ApiResponse(400, "Please try again!"));
            return Ok("Code is send!");
        }

        [HttpPost("verify-reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest(new ApiResponse(400, "user is not found!"));
            if (string.IsNullOrEmpty(user.OTPCode) || user.OTPCode != model.Code)
                return BadRequest(new ApiResponse(400, "code is invalid"));

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var result = await _userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
            //if (result.Succeeded is false)
            //    return BadRequest(new ApiResponse(400, "Please try again!"));
            user.OTPCode = "";
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(updateResult.Errors);
            }

            return Ok(new
            {
                Token = resetToken
            });
        }

        [HttpPost("reset-password-with-token")]
        public async Task<ActionResult<UserToReturnDto>> ResetPasswordWithToken(ResetPasswordWithTokenDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest(new ApiResponse(400, "user is not found!"));
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400, "Please try again!"));

            var token = await _authService.CreateTokenAsync(user, _userManager, false);
            var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);

            if (setToken.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            var role = await _userManager.GetRolesAsync(user);
            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);
            mapping.Role = role.FirstOrDefault() ?? "";
            mapping.Token = token;


            return Ok(mapping);
        }

        [Authorize]
        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Unauthorized(new ApiResponse(401,"لا يمكنك تغيير كلمة المرور"));
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400, "كلمة السر الحالية خطأ!"));
            return Ok(new ApiResponse(200, "تم تغيير كلمة السر بنجاح."));
        }



        [Authorize]
        [HttpGet("current")]
        public async Task<ActionResult<UserToReturnDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .Include(u => u.Technician)
                .Include(u=>u.Reviews)
                .FirstOrDefaultAsync(u => u.Email == email);


            var role = await _userManager.GetRolesAsync(user);

            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);

            mapping.Role = role.FirstOrDefault() ?? "";

            if (user.Technician is not null)
                mapping.IsActive = user.Technician.IsActive;

            return Ok(mapping);
        }


        [Authorize]
        [HttpPost("verify")]
        public async Task<ActionResult<UserToReturnDto>> VerifyAccount(string code)
        {
            if (code is null)
                return BadRequest(new ApiResponse(400));

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
                return BadRequest(new ApiResponse(400, "user is not found!"));

            if (user.OTPCode != code)
                return BadRequest(new ApiResponse(400, "code is false"));

            user.OTPCode = "";
            user.Vefify = true;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400));



            var token = await _authService.CreateTokenAsync(user, _userManager, false);
            var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);

            if (setToken.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            var role = await _userManager.GetRolesAsync(user);

            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);
            mapping.Role = role.FirstOrDefault() ?? "";
            mapping.Token = token;


            return Ok(mapping);
        }

        [Authorize]
        [HttpPost("resendcode")]
        public async Task<ActionResult<string>> ResendCode()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null || !user.Vefify)
                return BadRequest(new ApiResponse(400, "User not found or already verified."));

            var resetCode = new Random().Next(1000, 9999).ToString();

            var sendVerification = await _verificationCode.SendVerificationCode(user.Email, "OTP Code", $"Your verification code is: {resetCode}");

            if (!sendVerification) return BadRequest(new ApiResponse(400, "code has not send!"));

            user.OTPCode = resetCode;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded is false) return BadRequest(new ApiResponse(400, "Please try again!"));

            return Ok("Code is send!");

        }


        [Authorize]
        [HttpPost("addAddress")]
        public async Task<ActionResult<UpdateAddressDto>> AddAddressDto(UpdateAddressDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user is null)
                return Unauthorized(new ApiResponse(401, "User not found."));

            if (user.Addresses is not null)
            {
                bool duplicateExists = user.Addresses.Any(a =>
                             a.City.Equals(model.City, StringComparison.OrdinalIgnoreCase) &&
                             a.Center.Equals(model.Center, StringComparison.OrdinalIgnoreCase) &&
                             a.Street.Equals(model.Street, StringComparison.OrdinalIgnoreCase)

                        );

                if (duplicateExists)
                    return BadRequest(new ApiResponse(400, "Address already exists."));
            }


            var address = _mapper.Map<UpdateAddressDto, Address>(model);

            user.Addresses.Add(address);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400, "Failed to Add address."));


            return Ok(model);

        }

        [Authorize]
        [HttpPut("updateaddress/{id}")]
        public async Task<ActionResult> UpdateAddress(int id, UpdateAddressDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var specAddress = new AddressForUserSpecifications(id, user.Id);
            var address = await _unitOfWork.Repository<Address>().GetWithSpec(specAddress);

            if (address is null)
                return NotFound(new ApiResponse(404, "this Address is not not found"));



            if (address.City == model.City && address.Center == model.Center && address.Street == model.Street)
                return BadRequest(new ApiResponse(400));




            address.City = model.City;
            address.Center = model.Center;
            address.Street = model.Street;

            _unitOfWork.Repository<Address>().Update(address);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400));



            return Ok(new ApiResponse(200, "Address updated successfully."));
        }

        [Authorize]
        [HttpDelete("address/{id}")]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            //var user = await _userManager.Users
            //    .Include(u => u.Addresses)
            //    .FirstOrDefaultAsync(u => u.Email == email);
            var user = await _userManager.FindByEmailAsync(email);

            var specAddress = new AddressForUserSpecifications(id, user.Id);
            var address = await _unitOfWork.Repository<Address>().GetWithSpec(specAddress);

            if (address is null)
                return NotFound(new ApiResponse(404, "this Address is not not found"));

            _unitOfWork.Repository<Address>().Delete(address);
            var result = await _unitOfWork.CompleteAsync();

            if (result <= 0)
                return BadRequest(new ApiResponse(400));

            return NoContent();


        }

        [Authorize]
        [HttpPut("updateimage")]
        public async Task<ActionResult<UserToReturnDto>> UpdateImage(IFormFile image)
        {
            if (image == null)
                return BadRequest(new ApiResponse(400, "الرجاء إرسال صورة"));

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ApiResponse(404, "المستخدم غير موجود"));

            var folderName = "users/images";

            // حذف الصورة القديمة إذا كانت موجودة
            if (!string.IsNullOrEmpty(user.Image))
            {
                DocumentSettings.DeleteFile(user.Image, folderName);
            }

            // رفع الصورة الجديدة
            user.Image = DocumentSettings.UploadFile(image, folderName);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "حدثت مشكلة أثناء تحديث الصورة"));

            // التوكن
            var token = await _userManager.GetAuthenticationTokenAsync(user, "ServicesPlatform", "Token");
            if (token == null)
            {
                token = await _authService.CreateTokenAsync(user, _userManager, false);
                var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
                if (!setToken.Succeeded)
                    return Unauthorized(new ApiResponse(401));
            }

            var role = await _userManager.GetRolesAsync(user);

            return Ok(new UserToReturnDto
            {
                DisplayName = user.DisplayName,
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image ?? "",
                Role = role.FirstOrDefault() ?? "",
                Token = token,
            });
        }



        [Authorize]
        [HttpPut("deleteimage")]
        public async Task<ActionResult<UserToReturnDto>> DeleteUserImage()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (string.IsNullOrEmpty(user.Image))
                return BadRequest(new ApiResponse(400, "this user is haven't image!!"));

            DocumentSettings.DeleteFile(user.Image, "users\\images");

            user.Image = string.Empty;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400));

            var token = await _userManager.GetAuthenticationTokenAsync(user, "ServicesPlatform", "Token");
            if (token is null)
            {
                token = await _authService.CreateTokenAsync(user, _userManager, false);
                var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
                if (setToken.Succeeded is false)
                    return Unauthorized(new ApiResponse(401));
            }

            var role = await _userManager.GetRolesAsync(user);
            return Ok(new UserToReturnDto
            {
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image ?? "",
                Role = role.FirstOrDefault() ?? "",
                Token = token,

            });

        }


        [Authorize]
        [HttpPut("updateprofile")]
        public async Task<ActionResult<UserToReturnDto>> UpdateProfile(UpdateProfileDto model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            user.FullName = model.FullName;
            user.DisplayName = model.FirstName + " " + model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400, "Problem updating the profile"));
            var token = await _userManager.GetAuthenticationTokenAsync(user, "ServicesPlatform", "Token");
            if (token is null)
            {
                token = await _authService.CreateTokenAsync(user, _userManager, false);
                var setToken = await _userManager.SetAuthenticationTokenAsync(user, "ServicesPlatform", "Token", token);
                if (setToken.Succeeded is false)
                    return Unauthorized(new ApiResponse(401));
            }
            var role = await _userManager.GetRolesAsync(user);
            return Ok(new UserToReturnDto
            {
                DisplayName = user.DisplayName,
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image ?? "",
                Role = role.FirstOrDefault() ?? "",
                Token = token,
            });
        }


        [HttpGet("user-profile-by-id/{id}")]
        public async Task<ActionResult> GetUserProfileById(string id)
        {
            var user = await _userManager.Users
                  .Include(u => u.Addresses)
                  .Include(u => u.Orders)
                  .Include(u=>u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user is null)
                return NotFound(new ApiResponse(404, "User not found."));

            var mapping = _mapper.Map<AppUser, UserToReturnDto>(user);

            var role = await _userManager.GetRolesAsync(user);
            mapping.Role = role.FirstOrDefault() ?? "";

            var completedOrdersCount = user.Orders?.Count(o => o.Status == OrderStatus.Finished) ?? 0;
            var totalOrdersCount = user.Orders?.Count() ?? 0;
            var PendingOrdersCount = user.Orders?.Count(o => o.Status == OrderStatus.Pending) ?? 0;

            return Ok(new
            {
                User = mapping,
                CompletedOrders = completedOrdersCount,
                TotalOrders = totalOrdersCount,
                PendingOrders = PendingOrdersCount
            });

        }






        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }



    }
}
