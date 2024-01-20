using System.Net;
using Application.DTOs.Base;
using Application.DTOs.SubjectTopic;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/subject-topics")]
public class SubjectTopicController : Controller
{
    private readonly ISubjectTopicService _subjectTopicService;

    public SubjectTopicController(ISubjectTopicService subjectTopicService)
    {
        _subjectTopicService = subjectTopicService;
    }

    [HttpGet("get-all-subject-topic")]
    public async Task<IActionResult> GetAllSubjectTopics()
    {
        var subjects = await _subjectTopicService.GetAllSubjectTopics();

        var response = new ResponseDTO<List<SubjectTopicResponseDTO>>()
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subjects
        };

        return Ok(response);
    }
    
    [HttpGet("get-subject-topic/{subjectTopicId:int}")]
    public async Task<IActionResult> GetSubjectTopicsById(int subjectTopicId)
    {
        var subject = await _subjectTopicService.GetSubjectTopicsById(subjectTopicId);

        var response = new ResponseDTO<SubjectTopicResponseDTO>
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subject
        };

        return Ok(response);
    }
    
    [HttpPost("insert-subject-topic")]
    public async Task<IActionResult> InsertSubjectTopic(SubjectTopicRequestDTO subjectTopic)
    {
        await _subjectTopicService.AddSubjectTopics(subjectTopic);

        var result = await _subjectTopicService.GetAllSubjectTopics();
        
        var response = new ResponseDTO<List<SubjectTopicResponseDTO>>
        {
            Message = "Successfully Inserted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPut("update-subject-topic")]
    public async Task<IActionResult> UpdateSubjectTopic(SubjectTopicResponseDTO subjectTopic)
    {
        await _subjectTopicService.UpdateSubjectTopics(subjectTopic);

        var result = await _subjectTopicService.GetAllSubjectTopics();
        
        var response = new ResponseDTO<List<SubjectTopicResponseDTO>>()
        {
            Message = "Successfully Updated",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpDelete("delete-subject-topic/{subjectTopicId:int}")]
    public async Task<IActionResult> UpdateSubjectTopics(int subjectTopicId)
    {
        await _subjectTopicService.DeleteSubjectTopics(subjectTopicId);

        var result = await _subjectTopicService.GetAllSubjectTopics();
        
        var response = new ResponseDTO<List<SubjectTopicResponseDTO>>()
        {
            Message = "Successfully Deleted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}