using Application.DTOs.Content;
using Application.Interfaces.Services;
using Common.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RSOS.Controllers;

[Authentication]
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
    
    [HttpGet]
    public async Task<IActionResult> GetContentList(EContentRequestDTO content)
    {
        var contents = await _contentService.GetAllContents(content);

        contents.ContentsList = contents.ContentsList.Where(x => x.IsActive == content.IsActive).ToList();
        
        var data = ConvertViewToString("_ContentsList", contents, true);
        
        return Json(data);
    }

    public async Task<IActionResult> UploadContent(Contents contents)
    {
        var result = await _contentService.UpsertContents(contents);

        if (!result.Item1)
        {
            return Json(new
            {
                isSuccess = 0,
                message = result.Item2
            });
        }

        var contentModel = new EContentRequestDTO()
        {
            ClassId = contents.Class,
            SubjectId = contents.SubjectId,
            ContentType = contents.ContentType
        };
        
        var contentList = await _contentService.GetAllContents(contentModel);

        return Json(new
        {
            isSuccess = 1,
            message = result.Item2,
            htmlData = ConvertViewToString("_ContentsList", contentList, true)
        });
    }
    
    public async Task<IActionResult> UpdateContentStatus(int contentId, int classId, int subjectId, int contentType)
    {
        var result = await _contentService.UpdateContentStatus(contentId);

        if (!result.Item1)
        {
            return Json(new
            {
                isSucess = 0,
                message = "The following practice paper is linked to a specific questionnaire."
            });
        }
        
        var contentModel = new EContentRequestDTO()
        {
            ClassId = classId,
            SubjectId = subjectId,
            ContentType = contentType
        };
        
        var contentList = await _contentService.GetAllContents(contentModel);

        contentList.ContentsList = contentList.ContentsList
            .Where(x => x.IsActive == result.Item2)
            .OrderBy(x => x.ChapterNo)
            .ThenBy(x => x.PartNo)
            .ToList();
        
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

    [HttpGet]
    public async Task<IActionResult> GetContentById(int contentId, int classId, int subjectId, int contentType)
    {
        var subject = await _contentService.GetSubjectById(subjectId);
        
        var content = contentId == 0 ? new Contents()
        {
            Class = classId,
            SubjectId = subject.Id,
            SubjectName = subject.Title
        } : await _contentService.GetContentById(contentId);

        content.ContentType = contentType;
        
        return Json(new
        {
            htmlData = ConvertViewToString("_UploadEContents", content, true),
        });
    }
}