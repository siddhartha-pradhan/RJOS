using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Content;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/contents")]
public class ContentController : Controller
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }
    
    [HttpGet("get-content/{classId:int}/{subjectId:int}")]
    public async Task<IActionResult> GetContents(int classId, int subjectId)
    {
        var result = await _contentService.GetAllContents(classId, subjectId);

        var response = new ResponseDTO<List<ContentResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-content")]
    public async Task<IActionResult> PostContents(int? classId, int? subjectId)
    {
        var result = await _contentService.GetAllContents(classId, subjectId);

        var response = new ResponseDTO<List<ContentResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}