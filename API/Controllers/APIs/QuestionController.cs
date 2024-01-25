using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Question;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/questions")]
public class QuestionController : Controller
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }
    
    [HttpGet("get-all-questions/{classId:int}/{subjectId:int}")]
    public async Task<IActionResult> GetAllQuestions(int classId, int subjectId)
    {
        var result = await _questionService.GetAllQuestions(classId, subjectId);

        var response = new ResponseDTO<List<QuestionResponseDTO>>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }
}