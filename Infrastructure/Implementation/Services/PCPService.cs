using Application.DTOs.PCP;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class PCPService : IPCPService
{
    private readonly IGenericRepository _genericRepository;

    public PCPService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<PCPResponseDTO> GetPCPQuestionsByClass(int classId, int type)
    {
        var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.Class == classId);

        subjects = subjects.OrderBy(x => x.SubjectCode);

        var questions = new List<Question>();

        foreach (var subject in subjects)
        {
            var questionPapers =
                await _genericRepository.GetAsync<tblQuestion>(x => x.PaperType == type && x.SubjectId == subject.Id);
            
            questions.Add(new Question()
            {
                SubjectId = subject.SubjectCode ?? 0,
                SubjectName = subject.Title,
                PaperLastUploadedDate = "03-05-2024 03:45:00 PM"
            });
        }
        
        var result = new PCPResponseDTO()
        {
            PaperTypeId = type,
            PaperType = type == 1 ? "ePCP Final Paper" : "Practice Paper",
            Questions = questions
        };
        
        return result;
    }
}