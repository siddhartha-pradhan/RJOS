using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Question;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/questions")]
public class QuestionController : ControllerBase
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

        var response = new ResponseDTO<QuestionResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };
        
        return Ok(response);
    }

    [HttpPost("get-all-questions/")]
    public async Task<IActionResult> PostAllQuestions([FromForm] QuestionRequestDTO question)
    {
        var result = await _questionService.GetAllQuestions(question.ClassId, question.SubjectId);

        var response = new ResponseDTO<QuestionResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPost("get-all-questions-authorize")]
    public async Task<IActionResult> PostAllQuestionsAuthorize([FromForm] QuestionRequestDTO question)
    {
        var result = await _questionService.GetAllQuestions(question.ClassId, question.SubjectId);

        var response = new ResponseDTO<QuestionResponseDTO>
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }
}