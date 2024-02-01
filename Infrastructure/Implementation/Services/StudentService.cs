using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository _genericRepository;

    public StudentService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }
    
    public async Task<StudentResponseDTO> GetStudentRecords(int studentId)
    {
        var scores = await _genericRepository.GetAsync<tblStudentScore>(x => 
            x.StudentId == studentId && x.IsActive);

        var studentScores = scores.Select(x => new StudentScoreResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            Class = x.Class,
            Score = x.Score,
            SubjectId = x.StudentId,
            TopicId = x.TopicId,
            IsEdited = x.IsEdited,
            IsUploaded = x.IsUploaded,
        }).ToList();
        
        var responses = await _genericRepository.GetAsync<tblStudentResponse>(x => x.StudentId == studentId && x.IsActive);

        var studentResponses = responses.Select(x => new StudentResponsesResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            QuestionId = x.QuestionId,
            IsUploaded = x.IsUploaded,
            IsEdited = x.IsEdited,
            QuestionValue = x.QuestionValue,
            QuizGUID = x.QuizGUID,
        }).ToList();

        return new StudentResponseDTO()
        {
            StudentResponses = studentResponses,
            StudentScores = studentScores
        };
    }

    public async Task InsertStudentResponse(List<StudentResponseRequestDTO> studentResponse)
    {
        var result = studentResponse.Select(x => new tblStudentResponse
        {
            GUID = x.GUID,
            StudentId = x.StudentId,
            QuestionId = x.QuestionId,
            IsUploaded = x.IsUploaded,
            IsEdited = x.IsEdited,
            QuestionValue = x.QuestionValue,
            QuizGUID = x.QuizGUID,
            IsActive = true,
            CreatedBy = x.CreatedBy,
            CreatedOn = DateTime.Now,
        });

        await _genericRepository.AddMultipleEntityAsync(result);
    }
    
    public async Task<List<StudentResponsesResponseDTO>> GetStudentResponses(int studentId)
    {
        var result = await _genericRepository.GetAsync<tblStudentResponse>(x => x.StudentId == studentId && x.IsActive);

        return result.Select(x => new StudentResponsesResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            QuestionId = x.QuestionId,
            IsUploaded = x.IsUploaded,
            IsEdited = x.IsEdited,
            QuestionValue = x.QuestionValue,
            QuizGUID = x.QuizGUID,
        }).ToList();
    }

    public async Task InsertStudentScore(StudentScoreRequestDTO studentScore)
    {
        var result = new tblStudentScore()
        {
            GUID = studentScore.GUID,
            StudentId = studentScore.StudentId,
            Class = studentScore.Class,
            Score = studentScore.Score,
            SubjectId = studentScore.SubjectId,
            TopicId = studentScore.TopicId,
            IsEdited = studentScore.IsEdited,
            IsUploaded = studentScore.IsUploaded,
            IsActive = true,
            CreatedBy = studentScore.CreatedBy,
            CreatedOn = DateTime.Now
        };

        await _genericRepository.InsertAsync(result);
    }
}