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

    public async Task<QuestionResponseDTO> GetAllQuestions(int? classId, int? subjectId)
    {
        var result = new QuestionResponseDTO();
        
        var questions = await _genericRepository.GetAsync<tblQuestion>(x =>
            (!classId.HasValue || x.Class == classId) && 
            (!subjectId.HasValue || x.SubjectId == subjectId));

        var tblQuestions = questions as tblQuestion[] ?? questions.ToArray();
        
        result.Questions = tblQuestions.Select(x => new Question
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

        var flags = tblQuestions.Select(x => x.Flag);

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
        }).OrderBy(x => x.Id).ToList();
        
        return result;
    }
}