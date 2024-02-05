using Application.DTOs.Student;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    Task<StudentResponseDTO> GetStudentRecords(int studentId);

    Task InsertStudentResponse(List<StudentResponseRequestDTO> studentResponses);
    
    Task InsertStudentScore(List<StudentScoreRequestDTO> studentScores);
}