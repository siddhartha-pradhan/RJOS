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

    public async Task<QuestionResponseDTO> GetAllQuestions(int classId, int subjectId)
    {
        var result = new QuestionResponseDTO();
        
        var questions = await _genericRepository.GetAsync<tblQuestion>(x =>
            x.Class == classId && x.SubjectId == subjectId);

        result.Questions = questions.Select(x => new Question
        {
            Id = x.Id,
            QuestionTypeId = x.QuestionTypeId,
            Class = x.Class,
            SubjectId = x.SubjectId,
            TopicId = x.TopicId,
            IsMandatory = x.IsMandatory ? 1 : 0,
            Sequence = x.Sequence,
            QuestionValue = x.Question,
            Flag = x.Flag,
        }).ToList();

        var flags = questions.Select(x => x.Flag);

        var commons = await _genericRepository.GetAsync<tblCommon>(x => 
            flags.Contains(x.Flag));
        
        result.Commons = commons.Select(x => new Application.DTOs.Question.Common
        {
            Id = x.Id,
            Flag = x.Flag,
            CommonId = x.CommonId,
            Value = x.Value,
            LanguageId = x.LanguageId,
            Score = x.Score,
            CorrectAnswer = x.CorrectAnswer ?? 0,
        }).ToList();
        
        return result;
    }
}