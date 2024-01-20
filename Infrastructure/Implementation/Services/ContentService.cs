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
        throw new NotImplementedException();
    }
}