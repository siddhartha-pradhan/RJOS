using System.Security.Claims;
using Application.DTOs.Student;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Data.Implementation.Services;

public class StudentService : IStudentService
{
    private readonly IGenericRepository _genericRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public StudentService(IGenericRepository genericRepository, IHttpContextAccessor contextAccessor)
    {
        _genericRepository = genericRepository;
        _contextAccessor = contextAccessor;
    }

    public int StudentId
    {
        get
        {
            var userIdClaimValue = _contextAccessor.HttpContext?.User.FindFirstValue("studentid");

            return int.TryParse(userIdClaimValue, out var userId) ? userId : 0;
        }
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

    public async Task<StudentExamResponseDTO> GetStudentExamSubjects()
    {
        var pcpDate = (await _genericRepository.GetAsync<tblPCPDate>(x => x.IsActive)).MaxBy(x => x.Id);

        if (pcpDate == null)
        {
            return new StudentExamResponseDTO()
            {
                IsEligible = false,
                Message = "There are no any active ePCP dates allocated at the moment.",
                PCPEndDate = "",
                PCPStartDate = "",
                SubjectsList = [],
            };
        }

        if (pcpDate.StartDate <= DateTime.Now && DateTime.Now <= pcpDate.EndDate)
        {
            var studentId = StudentId;
        
            var subjects = await _genericRepository.GetAsync<tblSubject>();

            var studentVideoTrackingDetails = 
                await _genericRepository.GetAsync<tblStudentVideoTracking>(x => 
                    x.StudentId == studentId && x.PercentageCompleted >= 95 && x.VideoDurationInSeconds > 0);
        
            var studentScores = 
                await _genericRepository.GetAsync<tblStudentScore>(x => 
                    x.StudentId == studentId && x.TopicId == 0);
        
            var subjectIdsWithMatchingTracking = studentVideoTrackingDetails.Select(t => t.SubjectId).ToList();
        
            var subjectIdsWithZeroScore = studentScores.Select(s => s.SubjectId).ToList();

            var result = subjects.Where(subject =>
                    subjectIdsWithMatchingTracking.Contains(subject.Id) && !subjectIdsWithZeroScore.Contains(subject.Id))
                .Select(subject => new SubjectDetails
                {
                    Id = subject.Id,
                    Code = subject.SubjectCode ?? 0,
                    Class = subject.Class ?? 10,
                    Name = subject.Title
                }).ToList();
            
            return new StudentExamResponseDTO()
            {
                IsEligible = true,
                Message = $"The ePCP dates align with the allocated period of {pcpDate.StartDate:dd-MM-yyyy hh:mm:ss tt} to {pcpDate.EndDate:dd-MM-yyyy hh:mm:ss tt}. Hence you are eligible for the examination at the moment.",
                PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
                SubjectsList = result,
            };
        }
        
        return new StudentExamResponseDTO()
        {
            IsEligible = false,
            Message = $"The ePCP dates do not align with the allocated period of {pcpDate.StartDate:dd-MM-yyyy hh:mm:ss tt} to {pcpDate.EndDate:dd-MM-yyyy hh:mm:ss tt}. Hence you are not eligible for the examination at the moment.",
            PCPEndDate = pcpDate.StartDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
            PCPStartDate = pcpDate.EndDate.ToString("dd-MM-yyyy hh:mm:ss tt"),
            SubjectsList = [],
        };
    }
}