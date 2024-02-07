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
            SubjectId = x.SubjectId,
            TopicId = x.TopicId,
            IsEdited = x.IsEdited ? 1 : 0,
            IsUploaded = x.IsUploaded ? 1 : 0,
        }).ToList();
        
        var responses = await _genericRepository.GetAsync<tblStudentResponse>(x => x.StudentId == studentId && x.IsActive);

        var studentResponses = responses.Select(x => new StudentResponsesResponseDTO()
        {
            Id = x.Id,
            GUID = x.GUID,
            StudentId = x.StudentId,
            QuestionId = x.QuestionId,
            IsUploaded = x.IsUploaded ? 1 : 0,
            IsEdited = x.IsEdited ? 1 : 0,
            QuestionValue = x.QuestionValue,
            QuizGUID = x.QuizGUID,
        }).ToList();

        return new StudentResponseDTO()
        {
            StudentResponses = studentResponses,
            StudentScores = studentScores
        };
    }

    public async Task InsertStudentResponse(List<StudentResponseRequestDTO> studentResponses)
    {
        foreach (var studentResponse in studentResponses)
        {
            var response = await _genericRepository.GetFirstOrDefaultAsync<tblStudentResponse>(x =>
                x.GUID == studentResponse.GUID);

            if (response == null)
            {
                var studentResponseModel = new tblStudentResponse
                {
                    GUID = studentResponse.GUID,
                    QuizGUID = studentResponse.QuizGUID,
                    StudentId = studentResponse.StudentId,
                    QuestionId = studentResponse.QuestionId,
                    QuestionValue = studentResponse.QuestionValue,
                    IsEdited = studentResponse.IsEdited == 1,
                    IsUploaded = studentResponse.IsUploaded == 1,
                    IsActive = true,
                    CreatedBy = studentResponse.StudentId,
                    CreatedOn = DateTime.Now,
                };

                await _genericRepository.InsertAsync(studentResponseModel);
            }
            else
            {
                response.QuizGUID = studentResponse.QuizGUID;
                response.StudentId = studentResponse.StudentId;
                response.QuestionId = studentResponse.QuestionId;
                response.QuestionValue = studentResponse.QuestionValue;
                response.IsEdited = true;
                response.IsUploaded = studentResponse.IsUploaded == 1;
                response.LastUpdatedBy = studentResponse.StudentId;
                response.LastUpdatedOn = DateTime.Now;

                await _genericRepository.UpdateAsync(response);
            }
        }
    }

    public async Task InsertStudentScore(List<StudentScoreRequestDTO> studentScores)
    {
        foreach (var studentScore in studentScores)
        {
            var score = await _genericRepository.GetFirstOrDefaultAsync<tblStudentScore>(x =>
                x.GUID == studentScore.GUID);

            if (score == null)
            {
                var studentScoreModel = new tblStudentScore()
                {
                    GUID = studentScore.GUID,
                    StudentId = studentScore.StudentId,
                    Class = studentScore.Class,
                    SubjectId = studentScore.SubjectId,
                    TopicId = studentScore.TopicId,
                    Score = studentScore.Score ?? "0",
                    IsEdited = studentScore.IsEdited == 1,
                    IsUploaded = studentScore.IsUploaded == 1,
                    IsActive = true,
                    CreatedBy = studentScore.CreatedBy,
                    CreatedOn = DateTime.Now
                };

                await _genericRepository.InsertAsync(studentScoreModel);
            }
            else
            {
                score.StudentId = studentScore.StudentId;
                score.Class = studentScore.Class;
                score.SubjectId = studentScore.SubjectId;
                score.TopicId = studentScore.TopicId;
                score.Score = studentScore.Score ?? "0";
                score.IsEdited = studentScore.IsEdited == 1;
                score.IsUploaded = studentScore.IsUploaded == 1;
                score.LastUpdatedBy = studentScore.StudentId;
                score.LastUpdatedOn = DateTime.Now;
                
                await _genericRepository.UpdateAsync(score);

            }
        }
    }
}