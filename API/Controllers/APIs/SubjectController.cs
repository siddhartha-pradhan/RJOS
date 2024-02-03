using Application.DTOs.Base;
using Application.DTOs.Subject;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/subjects")]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet("get-all-subjects")]
    public async Task<IActionResult> GetAllSubject()
    {
        var result =  await _subjectService.GetAllSubjects();

        var response = new ResponseDTO<List<SubjectResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPost("get-all-subjects")]
    public async Task<IActionResult> PostAllSubject()
    {
        var result =  await _subjectService.GetAllSubjects();

        var response = new ResponseDTO<List<SubjectResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}

