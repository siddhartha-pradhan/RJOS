using Application.DTOs.EBook;

namespace Application.Interfaces.Services;

public interface IEBookService
{
    Task<List<EBookResponseDTO>> GetAllEBooks(int? classId, int? subjectCode, string? volume);
}