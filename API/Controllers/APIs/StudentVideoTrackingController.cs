using System.Net;
using Application.DTOs.Base;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Application.DTOs.Tracking;
using Microsoft.AspNetCore.Authorization;

namespace RJOS.Controllers.APIs;

[Route("api/student-video-tracking")]
[ApiController]
public class StudentVideoTrackingController : ControllerBase
{
    private readonly IStudentVideoTrackingService _studentVideoTrackingService;

    public StudentVideoTrackingController(IStudentVideoTrackingService studentVideoTracking)
    {
        _studentVideoTrackingService = studentVideoTracking;
    }

    [HttpPost("insert-student-video-tracking")]
    public async Task<IActionResult> InsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.InsertStudentVideoTracking(studentVideoTrackingRequest);

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
    [HttpPost("insert-student-video-tracking-authorize")]
    public async Task<IActionResult> InsertStudentVideoTrackingAuthorize(StudentVideoTrackingRequestDTO studentVideoTrackingRequest)
    {
        await _studentVideoTrackingService.InsertStudentVideoTracking(studentVideoTrackingRequest);

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

    [HttpPost("update-student-video-tracking")]
    public async Task<IActionResult> UpdateStudentVideoTracking(StudentVideoTrackingResponseDTO studentVideoTrackingResponse)
    {
        await _studentVideoTrackingService.UpdateStudentVideoTracking(studentVideoTrackingResponse);

        var response = new ResponseDTO<object>
        {
            Status = "Success",
            Message = "Successfully Updated",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }
    
    [Authorize]
    [HttpPost("update-student-video-tracking-authorize")]
    public async Task<IActionResult> UpdateStudentVideoTrackingAuthorize(StudentVideoTrackingResponseDTO studentVideoTrackingResponse)
    {
        await _studentVideoTrackingService.UpdateStudentVideoTracking(studentVideoTrackingResponse);

        var response = new ResponseDTO<object>
        {
            Status = "Success",
            Message = "Successfully Updated",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(response);
    }
}

