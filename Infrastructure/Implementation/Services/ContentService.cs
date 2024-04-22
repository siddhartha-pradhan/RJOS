using Application.DTOs.Content;
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

    public async Task<EContentResponseDTO> GetAllContents(ContentRequestDTO content)
    {
        var subject = content.SubjectId.HasValue ? await _genericRepository.GetByIdAsync<tblSubject>(content.SubjectId) : new tblSubject();
        
        var contentList = await _genericRepository.GetAsync<tblContent>(x => 
            (!content.ClassId.HasValue || x.Class == content.ClassId) && 
            (!content.SubjectId.HasValue || x.SubjectId == content.SubjectId) && x.IsActive);

        var result = contentList.OrderBy(x => x.Sequence).Select(x => new Contents()
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
            Sequence = x.Sequence ?? 0,
            SubjectCode = subject?.SubjectCode ?? 0
        }).ToList();

        var eContent = new EContentResponseDTO()
        {
            SubjectId = subject?.Id ?? 0,
            Class = content.ClassId ?? 10,
            SubjectCode = subject?.SubjectCode ?? 0,
            SubjectName = subject?.Title ?? "",
            ContentsList = result
        };
        
        return eContent;
    }

    public async Task<(bool, string)> UpsertContents(Contents content)
    {
        var existingContent = await _genericRepository.GetFirstOrDefaultAsync<tblContent>(x =>
            x.Class == content.Class && x.SubjectId == content.SubjectId &&
            x.ChapterNo == content.ChapterNo && x.PartNo == content.PartNo);

        if (existingContent != null)
        {
            return (false, "An existing eContent on the same subject and class exists with the same chapter number and part number");
        }
        
        if (content.Id == 0)
        {
            var contentModel = await _genericRepository.GetByIdAsync<tblContent>(content.Id);

            if (contentModel == null) return (false, "eContent not found.");
            
            contentModel.Faculty = content.Faculty;
            contentModel.ChapterName = content.ChapterName;
            contentModel.ChapterNo = content.ChapterNo;
            contentModel.Description = "RSOS Content Model";
            contentModel.PartName = content.PartName;
            contentModel.PartNo = content.PartNo;
            contentModel.TimeInSeconds = content.TimeInSeconds;
            contentModel.YouTubeLink = content.YouTubeLink;

            await _genericRepository.UpdateAsync(contentModel);

            return (true, "eContent successfully updated.");

        }
        else
        {
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
                Sequence = content.Sequence,
                IsActive = true,
            };

            await _genericRepository.InsertAsync(contentModel);

            return (true, "eContent successfully added.");
        }
    }

    public async Task<bool> UpdateContentStatus(int contentId)
    {
        var content = await _genericRepository.GetByIdAsync<tblContent>(contentId);

        if (content == null) return false;
        
        content.IsActive = !content.IsActive;

        await _genericRepository.UpdateAsync(content);

        return true;
    }

    public async Task<List<SubjectResponseDTO>> GetSubjectsByClass(int classId)
    {
        var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.Class == classId);

        var result = subjects.Select(x => new SubjectResponseDTO()
        {
            Id = x.Id,
            Class = x.Class ?? classId,
            Title = x.Title,
            SubjectCode = x.SubjectCode,
            TitleInHindi = x.TitleInHindi
        }).ToList();

        return result;
    }
}