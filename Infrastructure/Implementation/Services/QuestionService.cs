using Application.DTOs.Question;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class QuestionService : IQuestionService
{
    private readonly IGenericRepository _genericRepository;

    public QuestionService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<QuestionResponseDTO>> GetAllQuestions(int classId, int subjectId)
    {
        var result = new List<QuestionResponseDTO>();
        
        var questions = await _genericRepository.GetAsync<tblQuestion>(x =>
            x.Class == classId && x.SubjectId == subjectId);

        foreach (var question in questions)
        {
            var commons = await _genericRepository.GetAsync<tblCommon>(x => 
                x.Flag == question.Id);

            var commonResult = commons.Select(common => new QuestionCommonResponseDTO()
                {
                    Id = common.Id,
                    CommonId = common.CommonId,
                    Flag = common.Flag,
                    Value = common.Value,
                    LanguageId = common.LanguageId,
                    Score = common.Score
                }).ToList();
            
            var questionResponse = new QuestionResponseDTO
            {
                Id = question.Id,
                QuestionTypeId = question.QuestionTypeId,
                Class = question.Class,
                SubjectId = question.SubjectId,
                TopicId = question.TopicId,
                IsMandatory = question.IsMandatory,
                Sequence = question.Sequence,
                Question = question.Question,
                Flag = question.Flag,
                Commons = commonResult
            };
            result.Add(questionResponse);
        }

        return result;
    }
}