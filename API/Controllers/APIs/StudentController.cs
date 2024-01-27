using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Student;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/students")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpPost("insert-student-response")]
    public async Task<IActionResult> InsertStudentResponse(StudentResponsesRequestDTO studentResponse)
    {
        await _studentService.InsertStudentResponse(studentResponse);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }

    [HttpGet("get-student-responses/{studentId:int}")]
    public async Task<IActionResult> GetStudentResponses(int studentId)
    {
        var result = await _studentService.GetStudentResponses(studentId);
        
        var response = new ResponseDTO<List<StudentResponsesResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
    
    [HttpPost("insert-student-scores")]
    public async Task<IActionResult> InsertStudentScores(StudentScoreRequestDTO studentScore)
    {
        await _studentService.InsertStudentScore(studentScore);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }
    
    [HttpGet("get-student-scores/{studentId:int}")]
    public async Task<IActionResult> GetStudentScore(int studentId)
    {
        var result = await _studentService.GetStudentScore(studentId);
        
        var response = new ResponseDTO<List<StudentScoreResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
    
    [HttpPost("insert-login-details/{studentId}/{registrationToken}")]
    public async Task<IActionResult> InsertLoginDetails(int studentId, string registrationToken)
    {
        await _studentService.InsertLoginDetails(studentId, registrationToken);

        var result = new ResponseDTO<object>()
        {
            Status = "Success",
            Message = "Successfully Inserted",
            StatusCode = HttpStatusCode.OK,
            Result = true
        };

        return Ok(result);
    }
}