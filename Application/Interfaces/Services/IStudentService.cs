using Application.DTOs.Student;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    int StudentId { get; }

    string StudentDateOfBirth { get; }
    
    string StudentSSOID { get; }
    
    Task<StudentResponseDTO> GetStudentRecords(int studentId);

    Task InsertStudentResponse(List<StudentResponseRequestDTO> studentResponses);
    
    Task InsertStudentScore(List<StudentScoreRequestDTO> studentScores);

    Task<StudentExamResponseDTO> GetStudentExamSubjects(string secureToken);
}