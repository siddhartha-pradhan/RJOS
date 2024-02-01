using Application.DTOs.Student;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    Task<StudentResponseDTO> GetStudentRecords(int studentId);
    
    Task InsertStudentResponse(List<StudentResponseRequestDTO> studentResponse);
    
    Task InsertStudentScore(StudentScoreRequestDTO studentScore);
}