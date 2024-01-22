using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Enrollment;
using Application.Interfaces.Services;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/enrollments")]
public class EnrollmentController : Controller
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpGet("get-enrollment-status/{enrollmentId:int}")]
    public async Task<IActionResult> GetEnrollmentStatus(int enrollmentId)
    {
        var result = await _enrollmentService.GetEnrollmentStatus(enrollmentId);

        if (result.EnrollmentId == 0)
        {
            var invalidResponse = new ResponseDTO<EnrollmentResponseDTO>()
            {
                Status = "Not Found",
                Message = "Invalid Enrollment Identifier",
                StatusCode = HttpStatusCode.NotFound,
                Result = result
            };
            
            return NotFound(invalidResponse);
        }
        
        var response = new ResponseDTO<EnrollmentResponseDTO>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
}