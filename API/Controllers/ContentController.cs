using Application.DTOs.Content;
using Application.Interfaces.Services;
using ClosedXML.Excel;
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

    [HttpGet]
    public async Task<IActionResult> DownloadSheet(int classId, int subjectId, int contentType, bool activeStatus)
    {
        var contentModel = new EContentRequestDTO()
        {
            ClassId = classId,
            SubjectId = subjectId,
            ContentType = contentType
        };
        
        var contentList = await _contentService.GetAllContents(contentModel);

        contentList.ContentsList = contentList.ContentsList.Where(x => x.IsActive == activeStatus).ToList();
        
        var stream = CreateExcelFile(contentList.ContentsList);

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Contents.xlsx");
    }

    public MemoryStream CreateExcelFile(List<Contents> contents)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Contents");

        var headerRange = worksheet.Range("A1:I1");
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Font.FontName = "Arial";
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
        headerRange.Style.Font.FontColor = XLColor.DarkBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        worksheet.Column(1).Width = 20;
        worksheet.Column(2).Width = 20; 
        worksheet.Column(3).Width = 30; 
        worksheet.Column(4).Width = 15; 
        worksheet.Column(5).Width = 30; 
        worksheet.Column(6).Width = 10; 
        worksheet.Column(7).Width = 20; 
        worksheet.Column(8).Width = 30; 
        worksheet.Column(9).Width = 20; 

        worksheet.Cell("A1").Value = "Subject Code";
        worksheet.Cell("B1").Value = "Subject";
        worksheet.Cell("C1").Value = "Faculty";
        worksheet.Cell("D1").Value = "Chapter No";
        worksheet.Cell("E1").Value = "Chapter Name";
        worksheet.Cell("F1").Value = "Part No";
        worksheet.Cell("G1").Value = "Part Name";
        worksheet.Cell("H1").Value = "YouTube Link";
        worksheet.Cell("I1").Value = "Time In Seconds";

        for (var i = 0; i < contents.Count; i++)
        {
            var row = i + 2; 
            var content = contents[i];
            worksheet.Cell(row, 1).Value = content.SubjectCode;
            worksheet.Cell(row, 2).Value = content.SubjectName;
            worksheet.Cell(row, 3).Value = content.Faculty;
            worksheet.Cell(row, 4).Value = content.ChapterNo;
            worksheet.Cell(row, 5).Value = content.ChapterName;
            worksheet.Cell(row, 6).Value = content.PartNo;
            worksheet.Cell(row, 7).Value = content.PartName;
            worksheet.Cell(row, 8).Value = content.YouTubeLink;
            worksheet.Cell(row, 9).Value = content.TimeInSeconds;
            worksheet.Row(row).Style.Font.FontName = "Arial";
            
            for (var col = 1; col <= 9; col++)
            {
                worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            }
        }

        for (var col = 1; col <= 9; col++)
        {
            worksheet.Cell(1, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        
        var stream = new MemoryStream();
        
        workbook.SaveAs(stream);
        
        stream.Position = 0;
        
        return stream;
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