using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/students")]
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }
    
    [HttpGet("get-student-responses/{studentId:int}")]
    public async Task<IActionResult> GetStudentResponses(int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
    
    [HttpPost("get-student-responses")]
    public async Task<IActionResult> GetStudentResponsesResult([FromForm]int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);
        
        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-student-responses-authorize")]
    public async Task<IActionResult> GetStudentResponsesResultAuthorize([FromForm]int studentId)
    {
        var result = await _studentService.GetStudentRecords(studentId);

        var response = new ResponseDTO<StudentResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("insert-student-records")]
    public async Task<IActionResult> InsertStudentResponse(StudentRequestDTO studentResponse)
    {
        await _studentService.InsertStudentResponse(studentResponse.StudentResponse);

        await _studentService.InsertStudentScore(studentResponse.StudentScores);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted / Updated.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [Authorize]
    [HttpPost("insert-student-records-authorize")]
    public async Task<IActionResult> InsertStudentResponseAuthorize([FromForm]StudentRequestDTO studentResponse)
    {
        await _studentService.InsertStudentResponse(studentResponse.StudentResponse);

        await _studentService.InsertStudentScore(studentResponse.StudentScores);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted / Updated.",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }
}