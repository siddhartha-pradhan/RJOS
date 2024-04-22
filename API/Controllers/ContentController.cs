using Application.DTOs.Content;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSOS.Controllers;

public class ContentController : BaseController<ContentController>
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetContentList(ContentRequestDTO content)
    {
        var contents = await _contentService.GetAllContents(content);

        return PartialView("_ContentsList", contents);
    }

    public async Task<IActionResult> UploadContent(Contents contents)
    {
        var result = await _contentService.UpsertContents(contents);

        if (!result.Item1)
        {
            return Json(new
            {
                isSucess = 0,
                message = result.Item2
            });
        }

        var contentModel = new ContentRequestDTO()
        {
            ClassId = contents.Class,
            SubjectId = contents.SubjectId
        };
        
        var contentList = await _contentService.GetAllContents(contentModel);

        return Json(new
        {
            isSuccess = 1,
            message = result.Item2,
            htmlData = ConvertViewToString("_ContentsList", contentList, true)
        });
    }

    public async Task<IActionResult> UpdateContentStatus(int contentId, int classId, int subjectId)
    {
        var result = await _contentService.UpdateContentStatus(contentId);

        if (!result)
        {
            return Json(new
            {
                isSucess = 0,
                message = "eContent not found"
            });
        }
        
        var contentModel = new ContentRequestDTO()
        {
            ClassId = classId,
            SubjectId = subjectId
        };
        
        var contentList = await _contentService.GetAllContents(contentModel);

        return Json(new
        {
            isSuccess = 1,
            htmlData = ConvertViewToString("_ContentsList", contentList, true)
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetSubjectsByClass(int classId)
    {
        var subjectsList = await _contentService.GetSubjectsByClass(classId);

        var subject = new SelectList(subjectsList, "Id", "Value");

        return Json(subject);
    }
}