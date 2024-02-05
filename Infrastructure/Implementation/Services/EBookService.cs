using Application.DTOs.EBook;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class EBookService : IEBookService
{
    private readonly IGenericRepository _genericRepository;

    public EBookService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<EBookResponseDTO>> GetAllEBooks(int? classId, int? subjectCode, string? volume)
    {
        var ebooks = await _genericRepository.GetAsync<tblEbook>(x =>
            (!classId.HasValue || x.Class == classId) &&
            (!subjectCode.HasValue || x.CodeNo == subjectCode) &&
            (string.IsNullOrEmpty(volume) || x.Volume == volume) && x.IsActive);

        return ebooks.OrderBy(x => x.Sequence).Select(x => new EBookResponseDTO()
        {
            Id = x.Id,
            Code = x.CodeNo,
            Class = x.Class,
            Volume = x.Volume,
            FileName = x.FileName ?? "No PDF",
            NameOfBook = x.NameOfBook ?? ""
        }).ToList();
    }
}