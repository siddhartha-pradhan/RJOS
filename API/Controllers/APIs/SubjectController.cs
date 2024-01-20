using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Subject;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/subjects")]
public class SubjectController : Controller
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet("get-all-subjects")]
    public async Task<IActionResult> GetAllSubjects()
    {
        var subjects = await _subjectService.GetAllSubjects();

        var response = new ResponseDTO<List<SubjectResponseDTO>>()
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subjects
        };

        return Ok(response);
    }
    
    [HttpGet("get-subject/{subjectId:int}")]
    public async Task<IActionResult> GetSubjectById(int subjectId)
    {
        var subject = await _subjectService.GetSubjectById(subjectId);

        var response = new ResponseDTO<SubjectResponseDTO>
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subject
        };

        return Ok(response);
    }
    
    [HttpPost("insert-subject")]
    public async Task<IActionResult> InsertSubject(SubjectRequestDTO subject)
    {
        await _subjectService.AddSubject(subject);

        var result = await _subjectService.GetAllSubjects();
        
        var response = new ResponseDTO<List<SubjectResponseDTO>>()
        {
            Message = "Successfully Inserted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPut("update-subject")]
    public async Task<IActionResult> UpdateSubject(SubjectResponseDTO subject)
    {
        await _subjectService.UpdateSubject(subject);

        var result = await _subjectService.GetAllSubjects();
        
        var response = new ResponseDTO<List<SubjectResponseDTO>>()
        {
            Message = "Successfully Updated",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpDelete("delete-subject/{subjectId:int}")]
    public async Task<IActionResult> UpdateSubject(int subjectId)
    {
        await _subjectService.DeleteSubject(subjectId);

        var result = await _subjectService.GetAllSubjects();
        
        var response = new ResponseDTO<List<SubjectResponseDTO>>()
        {
            Message = "Successfully Deleted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}