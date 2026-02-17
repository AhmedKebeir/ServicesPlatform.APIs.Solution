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
using ServicesPlatform.Core.Specifications.Review_Specs;
using System.Security.Claims;

namespace ServicesPlatform.APIs.Controllers
{

    public class ReviewsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewsController(UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(ReviewRoReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("user-do-rating/{orderId}")]
        public async Task<ActionResult> CreateReview(int orderId, ReviewDto review)
        {
            if ( CheckReviewExists(review.OrderId, ReviewAuthorType.User).Result.Value)
                return BadRequest(new ApiResponse(400, "Review already exists for this order"));

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var order = await _unitOfWork.Repository<Order>().GetAsync(orderId);
            if (order is null || order.UserId != user.Id || order.Status != OrderStatus.Finished)
                return BadRequest(new ApiResponse(400, "Invalid order Id"));


            var reviewRepo = _unitOfWork.Repository<Review>();

            var reviewSpec = new GetReviewByOrderIdSpecifications(orderId, ReviewAuthorType.User);
            var existingReview = await reviewRepo.GetWithSpec(reviewSpec);

            if (existingReview is not null)
                return BadRequest(new ApiResponse(400, "Review already exists for this order"));

            var reviewMap = _mapper.Map<Review>(review);

            reviewMap.UserId = user.Id;
            reviewMap.AuthorId = user.Id;
            reviewMap.AuthorType = ReviewAuthorType.User;


            if (review.ImageUrl is not null)
                reviewMap.ImageUrl = DocumentSettings.UploadFile(review.ImageUrl, "Reviews");

            await reviewRepo.AddAsync(reviewMap);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create review"));

            var reviewToReturn = _mapper.Map<ReviewRoReturnDto>(reviewMap);
            return Ok(reviewToReturn);
        }

        [Authorize(Roles = "Technician")]
        [HttpPost("technician-do-rating/{orderId}")]
        public async Task<ActionResult> CreateReviewByTechnician(int orderId, TechReviewDto review)
        {
            if (CheckReviewExists(review.OrderId,ReviewAuthorType.Technician).Result.Value)
                return BadRequest(new ApiResponse(400, "Review already exists for this order"));

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user= await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            var order = await _unitOfWork.Repository<Order>().GetAsync(orderId);
            if (order is null || order.TechnicianId != user.Technician.Id || order.Status != OrderStatus.Finished)
                return BadRequest(new ApiResponse(400, "Invalid order Id"));

            var reviewRepo = _unitOfWork.Repository<Review>();

            var reviewSpec = new GetReviewByOrderIdAndTechnicianSpecifications(orderId);
            var existingReview = await reviewRepo.GetWithSpec(reviewSpec);

            if (existingReview is not null)
                return BadRequest(new ApiResponse(400, "Review already exists for this order"));

            var reviewMap = _mapper.Map<Review>(review);

            reviewMap.TechnicianId = user.Technician.Id;
            reviewMap.AuthorId = user.Technician.Id.ToString();
            reviewMap.AuthorType = ReviewAuthorType.Technician;

            if (review.ImageUrl is not null)
                reviewMap.ImageUrl = DocumentSettings.UploadFile(review.ImageUrl, "Reviews");

            await reviewRepo.AddAsync(reviewMap);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to create review"));

            var reviewToReturn = _mapper.Map<ReviewRoReturnDto>(reviewMap);
            return Ok(reviewToReturn);
        }

        //[Authorize]
        //[HttpGet]
        //public async Task<ActionResult<IReadOnlyList<ReviewRoReturnDto>>> GetUserReviews()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var user = await _userManager.FindByEmailAsync(email);
        //    var reviewSpec = new GetAllReviewForUserSpceifications(user.Id);
        //    var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewSpec);

        //    if (reviews is null || !reviews.Any())
        //        return NotFound(new ApiResponse(404, "No reviews found for the user"));

        //    var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);

        //    return Ok(reviewsToReturn);
        //}

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewRoReturnDto>> GetReviewById(int id)
        {

            var review = await _unitOfWork.Repository<Review>().GetAsync(id);
            if (review is null)
                return NotFound(new ApiResponse(404, "Review not found"));

            var reviewToReturn = _mapper.Map<ReviewRoReturnDto>(review);

            return Ok(reviewToReturn);
        }



        [Authorize(Roles = "Technician")]
        [HttpGet("my-reviews/{technicianId}")]
        public async Task<ActionResult<IReadOnlyList<ReviewRoReturnDto>>> GetTechnicianReviews(int technicianId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user?.Technician is null || user.Technician.Id != technicianId)
                return BadRequest(new ApiResponse(400, "Invalid technician Id"));

            var reviewSpec = new GetAllReviewForUserSpceifications(technicianId);
            var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewSpec);

            if (reviews is null || !reviews.Any())
                return NotFound(new ApiResponse(404, "No reviews found for the technician"));
            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);
            return Ok(reviewsToReturn);
        }

        [Authorize]
        [HttpGet("technician/{techId}")]
        public async Task<ActionResult<IReadOnlyList<ReviewRoReturnDto>>> GetReviewByTechId(int techId)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var reviewSpec = new GetAllReviewForUserSpceifications(techId);
            var reviews = await _unitOfWork.Repository<Review>().GetAllWithSpecAsync(reviewSpec);
            if (reviews is null)
                return NotFound(new ApiResponse(404, "No reviews found for the technician"));

            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);
            return Ok(reviewsToReturn);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteReview(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var review = await _unitOfWork.Repository<Review>().GetAsync(id);
            if (review is null || review.UserId != user.Id)
                return BadRequest(new ApiResponse(400, "Invalid review Id"));
            _unitOfWork.Repository<Review>().Delete(review);
            var result = await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return BadRequest(new ApiResponse(400, "Failed to delete review"));
            return Ok();
        }




        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<Pagination<ReviewRoReturnDto>>> GetAllReviewsWithUser([FromQuery] ReviewsSepcParams sepcParams)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            sepcParams.UserId = user.Id;
            var countSpec = new CountReviewsWithSpecParams(sepcParams);
            var reviewSpec = new GetAllReviewsSpceifications(sepcParams);

            var reviewRepo = _unitOfWork.Repository<Review>();

            var totalItems = await reviewRepo.GetCountAsync(countSpec);
            var reviews = await reviewRepo.GetAllWithSpecAsync(reviewSpec);


            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);

            return Ok(new Pagination<ReviewRoReturnDto>(
                         sepcParams.PageIndex,
                         sepcParams.PageSize,
                         reviewsToReturn,
                         totalItems
             ));
        }

        [Authorize]
        [HttpGet("technician")]
        public async Task<ActionResult<ReviewsPagination<ReviewRoReturnDto>>> GetAllReviewsWithTechnician([FromQuery] ReviewsSepcParams sepcParams)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.Users
                .Include(u => u.Technician)
                .FirstOrDefaultAsync(u => u.Email == email);
            sepcParams.TechnicianId = user.Technician.Id;
            var countSpec = new CountReviewsWithSpecParams(sepcParams,ReviewAuthorType.User);
            var reviewSpec = new GetAllReviewsSpceifications(sepcParams, ReviewAuthorType.User);

            var reviewRepo = _unitOfWork.Repository<Review>();

            var totalItems = await reviewRepo.GetCountAsync(countSpec);
            var reviews = await reviewRepo.GetAllWithSpecAsync(reviewSpec);


            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);

            var result = await GetTechnicianReviewsStatisticsAsync(user.Technician.Id);


            return Ok(new ReviewsPagination<ReviewRoReturnDto>
                (sepcParams.PageIndex,
                            sepcParams.PageSize,
                            reviewsToReturn,
                            totalItems,
                            result
                ));
        }


        [Authorize]
        [HttpGet("any-user")]
        public async Task<ActionResult<Pagination<ReviewRoReturnDto>>> GetAllReviewsAnyUserWithTechnician([FromQuery] ReviewsSepcParams sepcParams)
        {

            var countSpec = new CountReviewsWithSpecParams(sepcParams,ReviewAuthorType.User);
            var reviewSpec = new GetAllReviewsSpceifications(sepcParams,ReviewAuthorType.User);

            var reviewRepo = _unitOfWork.Repository<Review>();

            var totalItems = await reviewRepo.GetCountAsync(countSpec);
            var reviews = await reviewRepo.GetAllWithSpecAsync(reviewSpec);


            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);

            return Ok(new Pagination<ReviewRoReturnDto>(
                         sepcParams.PageIndex,
                         sepcParams.PageSize,
                         reviewsToReturn,
                         totalItems
             ));
        }




        [Authorize]
        [HttpGet("technician-count/{id}")]
        public async Task<ActionResult<ReviewsStatistics>> GetTechnicianReviewsCount(int id)
        {
            var allCountReviews = await GetTechnicianReviewsStatisticsAsync(id);

            return Ok(allCountReviews);
        }







        [HttpGet("check-review/{orderId}")]
        public async Task<ActionResult<bool>> CheckReviewExists(int orderId ,ReviewAuthorType reviewAuthor)
        {
            var reviewSpec = new GetReviewByOrderIdSpecifications(orderId,reviewAuthor);
            var existingReview = await _unitOfWork.Repository<Review>().GetWithSpec(reviewSpec);
            return Ok(existingReview is not null);
        }


        [HttpGet("check-tech-review/{orderId}")]




        private async Task<ReviewsStatistics> GetTechnicianReviewsStatisticsAsync(int technicianId)
        {
            var reviewsRepo = _unitOfWork.Repository<Review>();

            var param = new ReviewsSepcParams
            {
                TechnicianId = technicianId,
                
            };

            var spec = new GetAllReviewsSpceifications(param,ReviewAuthorType.User);
            var reviews = await reviewsRepo.GetAllWithSpecAsync(spec);

            var grouped = reviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            var avarageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);

            return new ReviewsStatistics
            {
                TotalReviews = reviews.Count,
                OneStar = grouped.GetValueOrDefault(1, 0),
                TwoStar = grouped.GetValueOrDefault(2, 0),
                ThreeStar = grouped.GetValueOrDefault(3, 0),
                FourStar = grouped.GetValueOrDefault(4, 0),
                FiveStar = grouped.GetValueOrDefault(5, 0),
                AverageRating = avarageRating
            };
        }




        // user reviews

        
        [Authorize]
        [HttpGet("user-reviews")]
        public async Task<ActionResult<Pagination<ReviewRoReturnDto>>> GetUserReviews([FromQuery]ReviewsSepcParams sepcParams)
        {

            var user = await _userManager.FindByIdAsync(sepcParams.UserId);
            if (user is null)
                return (NotFound(new ApiResponse(404, "المستخدم غير موجود!")));

            

            var countSpec = new CountReviewsWithSpecParams(sepcParams, ReviewAuthorType.Technician);
            var reviewSpec = new GetAllReviewsSpceifications(sepcParams, ReviewAuthorType.Technician);

            var reviewRepo = _unitOfWork.Repository<Review>();

            var totalItems = await reviewRepo.GetCountAsync(countSpec);
            var reviews = await reviewRepo.GetAllWithSpecAsync(reviewSpec);


            var reviewsToReturn = _mapper.Map<IReadOnlyList<ReviewRoReturnDto>>(reviews);

            return Ok(new Pagination<ReviewRoReturnDto>(
                         sepcParams.PageIndex,
                         sepcParams.PageSize,
                         reviewsToReturn,
                         totalItems
             ));


        }

        [Authorize]
        [HttpGet("user-count/{id}")]
        public async Task<ActionResult<ReviewsStatistics>> GetUserReviewsCount(string id)
        {
            var allCountReviews = await GetUserReviewsStatisticsAsync(id);

            return Ok(allCountReviews);
        }



        private async Task<ReviewsStatistics> GetUserReviewsStatisticsAsync(string userId)
        {
            var reviewsRepo = _unitOfWork.Repository<Review>();

            var param = new ReviewsSepcParams
            {
                UserId = userId,

            };

            var spec = new GetAllReviewsSpceifications(param, ReviewAuthorType.Technician);
            var reviews = await reviewsRepo.GetAllWithSpecAsync(spec);

            var grouped = reviews
                .GroupBy(r => r.Rating)
                .ToDictionary(g => g.Key, g => g.Count());

            var avarageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating);

            return new ReviewsStatistics
            {
                TotalReviews = reviews.Count,
                OneStar = grouped.GetValueOrDefault(1, 0),
                TwoStar = grouped.GetValueOrDefault(2, 0),
                ThreeStar = grouped.GetValueOrDefault(3, 0),
                FourStar = grouped.GetValueOrDefault(4, 0),
                FiveStar = grouped.GetValueOrDefault(5, 0),
                AverageRating = avarageRating
            };
        }


    }
}
