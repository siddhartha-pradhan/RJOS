using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Tracking;
using Microsoft.AspNetCore.Authorization;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/student-video-tracking")]
public class StudentVideoTrackingController : ControllerBase
{
    private readonly IStudentVideoTrackingService _studentVideoTrackingService;

    public StudentVideoTrackingController(IStudentVideoTrackingService studentVideoTracking)
    {
        _studentVideoTrackingService = studentVideoTracking;
    }

    [HttpPost("upsert-student-video-tracking")]
    public async Task<IActionResult> UpsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.UpsertStudentVideoTracking(studentVideoTrackingRequest);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("upsert-student-video-tracking-authorize")]
    public async Task<IActionResult> UpsertStudentVideoTrackingAuthorize(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.UpsertStudentVideoTracking(studentVideoTrackingRequest);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [HttpGet("get-student-video-tracking/{studentId:int}")]
    public async Task<IActionResult> GetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-student-video-tracking")]
    public async Task<IActionResult> PostGetStudentVideoTrackingById(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("get-student-video-tracking-authorize")]
    public async Task<IActionResult> PostGetStudentVideoTrackingByIdAuthorize(int studentId)
    {
        var result = await _studentVideoTrackingService.GetStudentVideoTrackingByStudentId(studentId);

        var response = new ResponseDTO<List<StudentVideoTrackingResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}

