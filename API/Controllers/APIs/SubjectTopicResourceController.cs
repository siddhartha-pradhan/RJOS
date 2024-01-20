using System.Net;
using Application.DTOs.Base;
using Application.DTOs.SubjectTopicResource;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/subject-topics-resources")]
public class SubjectTopicResourceController : Controller
{
    private readonly ISubjectTopicResourceService _subjectTopicResourceService;

    public SubjectTopicResourceController(ISubjectTopicResourceService subjectTopicResourceService)
    {
        _subjectTopicResourceService = subjectTopicResourceService;
    }

    [HttpGet("get-all-subject-topic-resources")]
    public async Task<IActionResult> GetAllSubjectTopicsResources()
    {
        var subjectResources = await _subjectTopicResourceService.GetAllSubjectTopicResource();

        var response = new ResponseDTO<List<SubjectTopicResourceResponseDTO>>()
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subjectResources
        };

        return Ok(response);
    }
    
    [HttpGet("get-subject-topic-resources/{subjectTopicId:int}")]
    public async Task<IActionResult> GetSubjectTopicsResourceById(int subjectTopicId)
    {
        var subjectResource = await _subjectTopicResourceService.GetSubjectTopicResourceById(subjectTopicId);

        var response = new ResponseDTO<SubjectTopicResourceResponseDTO>
        {
            Message = "Successfully Retrieved",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = subjectResource
        };

        return Ok(response);
    }
    
    [HttpPost("insert-subject-topic-resources")]
    public async Task<IActionResult> InsertSubjectTopicResource(SubjectTopicResourceRequestDTO subjectTopicResource)
    {
        await _subjectTopicResourceService.AddSubjectTopicResource(subjectTopicResource);

        var result = await _subjectTopicResourceService.GetAllSubjectTopicResource();
        
        var response = new ResponseDTO<List<SubjectTopicResourceResponseDTO>>
        {
            Message = "Successfully Inserted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpPut("update-subject-topic-resources")]
    public async Task<IActionResult> UpdateSubjectTopicResource(SubjectTopicResourceResponseDTO subjectTopicResource)
    {
        await _subjectTopicResourceService.UpdateSubjectTopicResource(subjectTopicResource);

        var result = await _subjectTopicResourceService.GetAllSubjectTopicResource();
        
        var response = new ResponseDTO<List<SubjectTopicResourceResponseDTO>>()
        {
            Message = "Successfully Updated",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
    
    [HttpDelete("delete-subject-topic-resources/{subjectTopicResourceId:int}")]
    public async Task<IActionResult> UpdateSubjectTopics(int subjectTopicResourceId)
    {
        await _subjectTopicResourceService.DeleteSubjectTopicResource(subjectTopicResourceId);

        var result = await _subjectTopicResourceService.GetAllSubjectTopicResource();
        
        var response = new ResponseDTO<List<SubjectTopicResourceResponseDTO>>()
        {
            Message = "Successfully Deleted",
            Status = "Success",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}