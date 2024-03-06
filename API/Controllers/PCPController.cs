using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace RSOS.Controllers;

public class PCPController : BaseController<PCPController>
{
    private readonly IPCPService _pcpService;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PCPController(IPCPService pcpService, IWebHostEnvironment webHostEnvironment)
    {
        _pcpService = pcpService;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetClassQuestions(int classId, int type)
    {
        var questions = await _pcpService.GetPCPQuestionsByClass(classId, type);
        
        return PartialView("_QuestionsList", questions);
    }
    
    public async Task<IActionResult> DownloadQuestionTemplate(int type)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;

        var questionSheet = type == 1 ? "ePCP Questions Format Sheet.xlsx" : "Practice Paper Questions Format Sheet.xlsx";
        
        var filePath = Path.Combine(wwwRootPath, "documents", "templates", questionSheet);
        
        if (!System.IO.File.Exists(filePath)) return NotFound();
            
        var memory = new MemoryStream();
        
        await using(var stream = new FileStream(filePath, FileMode.Open)) 
        {
            await stream.CopyToAsync(memory);
        }
        
        memory.Position = 0;
        
        return File(memory, GetContentType(filePath), questionSheet);
    }

    private static string GetContentType(string path) {
        var provider = new FileExtensionContentTypeProvider();
        
        if (!provider.TryGetContentType(path, out var contentType)) {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }
}