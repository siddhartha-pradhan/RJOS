using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Students;
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

    [HttpGet("get-all-students")]
    public async Task<IActionResult> GetAllStudents()
    {
        var students = await _studentService.GetAllStudents();

        var response = new ResponseDTO<List<StudentResponseDTO>>()
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = students
        };

        return Ok(response);
    }
    
    [HttpGet("get-student/{studentId:int}")]
    public async Task<IActionResult> GetStudentById(int studentId)
    {
        var student = await _studentService.GetStudentById(studentId);

        var response = new ResponseDTO<StudentResponseDTO>
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = student
        };

        return Ok(response);
    }
    
    [HttpPost("insert-student")]
    public async Task<IActionResult> InsertStudent(StudentRequestDTO student)
    {
        await _studentService.AddStudent(student);

        var result = await _studentService.GetAllStudents();
        
        var response = new ResponseDTO<List<StudentResponseDTO>>()
        {
            Message = "Successfully Inserted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPut("update-student")]
    public async Task<IActionResult> UpdateStudent(StudentResponseDTO student)
    {
        await _studentService.UpdateStudent(student);

        var result = await _studentService.GetAllStudents();
        
        var response = new ResponseDTO<List<StudentResponseDTO>>()
        {
            Message = "Successfully Updated",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpDelete("delete-student/{studentId:int}")]
    public async Task<IActionResult> UpdateStudent(int studentId)
    {
        await _studentService.DeleteStudent(studentId);

        var result = await _studentService.GetAllStudents();
        
        var response = new ResponseDTO<List<StudentResponseDTO>>()
        {
            Message = "Successfully Deleted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}