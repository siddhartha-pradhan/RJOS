using Application.DTOs.Base;
using Application.DTOs.Subject;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/subjects")]
[IgnoreAntiforgeryToken]
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [Authorize]
    [HttpPost("get-all-subjects")]
    public async Task<IActionResult> PostAllSubject([FromForm]SubjectRequestDTO subject)
    {
        var result =  await _subjectService.GetAllSubjects(subject.ClassId);

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

