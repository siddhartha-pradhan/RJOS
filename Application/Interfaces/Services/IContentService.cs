using Application.DTOs.Content;

namespace Application.Interfaces.Services;

public interface IContentService
{
    Task<List<ContentResponseDTO>> GetAllContents(int classId, int subjectId);
}