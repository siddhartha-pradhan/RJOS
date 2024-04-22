using Application.DTOs.Content;
using Application.DTOs.Dropdown;
using Application.DTOs.Subject;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class ContentService : IContentService
{
    private readonly IGenericRepository _genericRepository;

    public ContentService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<ContentResponseDTO>> GetAllContents(int? classId, int? subjectId)
    {
        var subject = subjectId.HasValue ? await _genericRepository.GetByIdAsync<tblSubject>(subjectId) : new tblSubject();
        
        var content = await _genericRepository.GetAsync<tblContent>(x => 
            (!classId.HasValue || x.Class == classId) && 
            (!subjectId.HasValue || x.SubjectId == subjectId) && x.IsActive);

        var result = content.OrderBy(x => x.Sequence).Select(x => new ContentResponseDTO()
        {
            Id = x.Id,
            Class = x.Class,
            SubjectId = x.SubjectId,
            SubjectName = subject?.Title ?? "",
            Faculty = x.Faculty ?? "",
            ChapterName = x.ChapterName,
            Description = x.Description,
            PartName = x.PartName,
            ChapterNo = x.ChapterNo,
            PartNo = x.PartNo,
            TimeInSeconds = x.TimeInSeconds,
            YouTubeLink = x.YouTubeLink,
        }).ToList();

        return result;
    }

    public async Task<EContentResponseDTO> GetAllContents(EContentRequestDTO content)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(content.SubjectId);
        
        var contentList = await _genericRepository.GetAsync<tblContent>(x => 
            x.Class == content.ClassId && 
            x.SubjectId == content.SubjectId);

        if (content.ContentType != 0)
        {
            contentList = content.ContentType == 1 ? 
                contentList.Where(x => x.YouTubeLink != "-") : 
                contentList.Where(x => x.YouTubeLink == "-");
        }

        contentList = contentList.OrderBy(x => x.ChapterNo)
            .ThenBy(x => x.PartNo);

        var result = new List<Contents>();
        
        foreach (var contentItem in contentList)
        {
            result.Add(new Contents()
            {
                Id = contentItem.Id,
                Class = contentItem.Class,
                SubjectId = contentItem.SubjectId,
                SubjectName = subject?.Title ?? "",
                Faculty = contentItem.Faculty ?? "",
                ChapterName = contentItem.ChapterName,
                Description = contentItem.Description,
                PartName = contentItem.PartName,
                ChapterNo = contentItem.ChapterNo,
                PartNo = contentItem.PartNo,
                TimeInSeconds = contentItem.TimeInSeconds,
                YouTubeLink = contentItem.YouTubeLink,
                Sequence = contentItem.Sequence ?? 0,
                SubjectCode = subject?.SubjectCode ?? 0,
                IsActive = contentItem.IsActive,
                IsDeletable = content.ContentType == 1 || await _genericRepository.GetFirstOrDefaultAsync<tblQuestion>(z => z.TopicId == contentItem.Id) == null
            });
        }

        var eContent = new EContentResponseDTO()
        {
            SubjectId = subject!.Id,
            Class = content.ClassId,
            SubjectCode = subject.SubjectCode ?? 0,
            SubjectName = subject.Title ?? "",
            ContentsList = result,
            ContentType = content.ContentType
        };
        
        return eContent;
    }

    public async Task<(bool, string)> UpsertContents(Contents content)
    {
        if (content.YouTubeLink == "-" || content.TimeInSeconds == 0)
        {
            return (false, "Please insert a valid YouTube link and time frame in seconds for the link.");
        }
        
        if (content.Id != 0)
        {
            var contentModel = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == content.Id);

            if (contentModel == null) return (false, "eContent not found.");
            
            contentModel.Faculty = content.Faculty;
            contentModel.ChapterName = content.ChapterName;
            contentModel.Description = "RSOS Content Model";
            contentModel.PartName = content.PartName;
            contentModel.TimeInSeconds = content.TimeInSeconds;
            contentModel.YouTubeLink = content.YouTubeLink;

            await _genericRepository.UpdateAsync(contentModel);

            return (true, "eContent successfully updated.");
        }
        else
        {
            var existingContent = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
                x.Class == content.Class && x.SubjectId == content.SubjectId &&
                x.ChapterNo == content.ChapterNo && x.PartNo == content.PartNo);

            if (existingContent != null)
            {
                return (false, "An existing eContent on the same subject and class exists with the same chapter number and part number");
            }

            var contents =
                await _genericRepository.GetAsync<tblContent>(x =>
                    x.Class == content.Class && x.SubjectId == content.SubjectId);

            var contentsList = contents as tblContent[] ?? contents.ToArray();
            
            var maxSequence = contentsList.Any() ? contentsList.Select(x => x.Sequence).Max() ?? 0 : 0;
            
            var contentModel = new tblContent()
            {
                Faculty = content.Faculty,
                ChapterName = content.ChapterName,
                ChapterNo = content.ChapterNo,
                Description = "RSOS Content Model",
                PartName = content.PartName,
                PartNo = content.PartNo,
                TimeInSeconds = content.TimeInSeconds,
                YouTubeLink = content.YouTubeLink,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
                SubjectId = content.SubjectId,
                Class = content.Class,
                Sequence = maxSequence + 5,
                IsActive = true,
            };

            await _genericRepository.InsertAsync(contentModel);

            return (true, "eContent successfully added.");
        }
    }

    public async Task<(bool, bool)> UpdateContentStatus(int contentId)
    {
        var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == contentId);

        if (content == null) return (false, false);

        if (content.YouTubeLink == "-")
        {
            var questionPaper = await _genericRepository.GetFirstOrDefaultAsync<tblQuestion>(x => x.TopicId == content.Id);

            if (questionPaper != null) return (false, false);
        }
        
        content.IsActive = !content.IsActive;

        await _genericRepository.UpdateAsync(content);

        return (true, !content.IsActive);
    }

    public async Task<List<SelectListModel>> GetSubjectsByClass(int classId)
    {
        var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.Class == classId);

        var result = subjects.Select(x => new SelectListModel()
        {
            Id = x.Id,
            Value = $"({x.SubjectCode}) {x.Title}"
        }).ToList();

        return result;
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectId);

        return new SubjectResponseDTO()
        {
            Id = subject!.Id,
            Class = subject.Class ?? 0,
            SubjectCode = subject.SubjectCode,
            Title = subject.Title,
            TitleInHindi = subject.TitleInHindi
        };
    }
    
    public async Task<Contents> GetContentById(int contentId)
    {
        var content = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x => x.Id == contentId);

        var subject = await _genericRepository.GetByIdAsync<tblSubject>(content!.SubjectId);
        
        var result = new Contents()
        {
            Id = content.Id,
            Class = content.Class,
            SubjectId = content.SubjectId,
            SubjectName = subject!.Title,
            SubjectCode = subject.SubjectCode ?? 0,
            ChapterName = content.ChapterName,
            ChapterNo = content.ChapterNo,
            PartNo = content.PartNo,
            TimeInSeconds = content.TimeInSeconds,
            Sequence = content.Sequence ?? 0,
            PartName = content.PartName,
            Faculty = content.Faculty ?? "",
            YouTubeLink = content.YouTubeLink,
            ContentType = content.YouTubeLink == "-" ? 2 : 1
        };

        return result;
    }
}