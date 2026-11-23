using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OverheadCraneGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppBaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Request successful")
        {
            return Ok(new ApiResponse<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = message,
                Data = data
            });
        }

        protected IActionResult Failure(string message, IEnumerable<string>? errors = null, int statusCode = StatusCodes.Status400BadRequest)
        {
            return StatusCode(statusCode, new ErrorResponse<object>
            {
                Success = false,
                Message = message,
                Errors = errors
            });
        }
    }
}
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
public class ErrorResponse<T> : ApiResponse<T>
{
    public IEnumerable<string>? Errors { get; set; }
}