using Application.DTOs.Content;
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

    public async Task<List<ContentResponseDTO>> GetAllContents(int classId, int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectId);
        
        var content = await _genericRepository.GetAsync<tblContent>(x => x.Class == classId && x.SubjectId == subjectId);

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
            YouTubeLink = x.YouTubeLink
        }).ToList();

        return result;
    }
}