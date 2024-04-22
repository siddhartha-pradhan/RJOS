using Application.DTOs.Content;
using Application.DTOs.Subject;

namespace Application.Interfaces.Services;

public interface IContentService
{
    Task<List<ContentResponseDTO>> GetAllContents(int? classId, int? subjectId);

    Task<EContentResponseDTO> GetAllContents(ContentRequestDTO content);

    Task<(bool, string)> UpsertContents(Contents content);

    Task<bool> UpdateContentStatus(int contentId);

    Task<List<SubjectResponseDTO>> GetSubjectsByClass(int classId);
}