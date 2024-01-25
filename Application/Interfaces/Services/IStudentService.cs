using Application.DTOs.Student;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    Task InsertStudentResponse(StudentResponsesRequestDTO studentResponse);
    
    Task<List<StudentResponsesResponseDTO>> GetStudentResponses(int studentId);
    
    Task InsertStudentScore(StudentScoreRequestDTO studentScore);
    
    Task<List<StudentScoreResponseDTO>> GetStudentScore(int studentId);
}