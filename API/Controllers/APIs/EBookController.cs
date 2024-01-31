using System.Net;
using Application.DTOs.Base;
using Application.DTOs.Content;
using Application.DTOs.EBook;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RJOS.Controllers.APIs;

[ApiController]
[Route("api/ebooks")]
public class EBookController : Controller
{
    private readonly IEBookService _eBookService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public EBookController(IEBookService eBookService, IWebHostEnvironment webHostEnvironment)
    {
        _eBookService = eBookService;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("get-all-ebooks")]
    public async Task<IActionResult> GetAllEbooks(int? classId, int? subjectCode, string? volume)
    {
        var result = await _eBookService.GetAllEBooks(classId, subjectCode, volume);
        
        var response = new ResponseDTO<List<EBookResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpPost("get-all-ebooks")]
    public async Task<IActionResult> PostAllEbooks(int? classId, int? subjectId, string? volume)
    {
        var result = await _eBookService.GetAllEBooks(classId, subjectId, volume);

        var response = new ResponseDTO<List<EBookResponseDTO>>()
        {
            Status = "Success",
            Message = "Successfully Retrieved",
            StatusCode = HttpStatusCode.OK,
            Result = result
        };

        return Ok(response);
    }

    [HttpGet("download-ebook/{fileUrl}")]
    public async Task<IActionResult> DownloadEbook(string fileUrl)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        
        var documentsFolderPath = Path.Combine(wwwRootPath, "documents");

        var ebooksFolderPath = Path.Combine(documentsFolderPath, "ebooks");
        
        var filePath = Path.Combine(ebooksFolderPath, fileUrl);
        
        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "EBook Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };
        
        if (!System.IO.File.Exists(filePath)) return NotFound(notFound);
        
        var memory = new MemoryStream();
        
        await using(var stream = new FileStream(filePath, FileMode.Open)) 
        {
            await stream.CopyToAsync(memory);
        }
        
        memory.Position = 0;
        
        return File(memory, GetContentType(filePath), fileUrl);
    }

    [HttpPost("download-ebook/{fileUrl}")]
    public async Task<IActionResult> PostDownloadEbook(string fileUrl)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;

        var documentsFolderPath = Path.Combine(wwwRootPath, "documents");

        var ebooksFolderPath = Path.Combine(documentsFolderPath, "ebooks");

        var filePath = Path.Combine(ebooksFolderPath, fileUrl);

        var notFound = new ResponseDTO<object>()
        {
            Status = "Not Found",
            Message = "EBook Not Found.",
            StatusCode = HttpStatusCode.NotFound,
            Result = false
        };

        if (!System.IO.File.Exists(filePath)) return NotFound(notFound);

        var memory = new MemoryStream();

        await using (var stream = new FileStream(filePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }

        memory.Position = 0;

        return File(memory, GetContentType(filePath), fileUrl);
    }

    private static string GetContentType(string path) {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType)) {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
}