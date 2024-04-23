using Application.DTOs.EBook;
using Application.DTOs.EContentBooks;
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
    
    public async Task<List<EContentBookResponseDTO>> GetAllEBooks(int classId, bool isActive)
    {
        var allSubjects = await _genericRepository.GetAsync<tblSubject>(x=> x.Class == classId);

        var ebooks = await _genericRepository.GetAsync<tblEbook>(
            x => (x.Class == classId) && x.IsActive == isActive);

        var combinedList = allSubjects
            .Select(subject => new
            {
                Subject = subject,
                Ebooks = ebooks.Where(ebook => ebook.CodeNo == subject.Id).ToList()
            })
            .SelectMany(result => result.Ebooks.DefaultIfEmpty(), (result, ebook) => new EContentBookResponseDTO
            {
                Id = ebook?.Id ?? 0,
                SubjectName = result?.Subject.Title ?? "",
                NameOfBook = ebook?.NameOfBook ?? "-",
                Volume = ebook?.Volume ?? "-",
                FileName = ebook?.FileName ?? "-",
                IsActive = ebook?.IsActive ?? false
            }).ToList();

        return combinedList;
    }

    public async Task<bool> UploadEContentBook(EContentBookRequestDTO eBookRequest)
    {
        var existingEBook = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x =>
            x.CodeNo == eBookRequest.SubjectId && x.Class == eBookRequest.ClassId && x.Volume == eBookRequest.Volume);

        if (existingEBook != null) return false;
        
        var model = new tblEbook
        {
            CodeNo = eBookRequest.SubjectId ?? 0,
            NameOfBook = eBookRequest.NameOfBook,
            Volume = eBookRequest.Volume ?? "",
            FileName = eBookRequest.EBookFile.FileName ,
            Class = eBookRequest.ClassId ?? 0,
            IsActive = true,
            CreatedBy = eBookRequest.CreatedBy ?? 0,
            CreatedOn = DateTime.Now,
        };

        await _genericRepository.InsertAsync(model);

        return true;
    }

    public async Task DeleteEBook(int ebookId)
    {
        var data = await _genericRepository.GetFirstOrDefaultAsync<tblEbook>(x => x.Id == ebookId);

        if (data != null)
        {
            data.IsActive = !data.IsActive;
            
            await _genericRepository.UpdateAsync(data);
        }
    }
}