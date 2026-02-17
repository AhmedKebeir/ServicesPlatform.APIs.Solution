using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesPlatform.APIs.Errors;
using ServicesPlatform.Repositories.Data;

namespace ServicesPlatform.APIs.Controllers
{
    
    public class BuggyController : BaseApiController
    {
        private readonly AppIdentityDbContext _dbContext;

        public BuggyController(AppIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet("notfound")]
        public ActionResult GetNoutFoundRequest()
        {
            var product = _dbContext.Technicianes.Find(1000000);
            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }

        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var product = _dbContext.Technicianes.Find(1000000);

            var productToReturn = product.ToString();

            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }
        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

        [HttpGet("unauthorized")]
        public ActionResult GetUnauthorizedError()
        {
            return Unauthorized(new ApiResponse(401));
        }
    }
}
