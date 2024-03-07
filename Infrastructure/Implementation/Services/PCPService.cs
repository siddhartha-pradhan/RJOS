using Application.DTOs.PCP;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using ClosedXML.Excel;

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
            var questionSheets =
                await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                    x.SubjectId == subject.Id && x.PaperType == type);
            
            questions.Add(new Question()
            {
                SubjectId = subject.Id,
                SubjectCode = subject.SubjectCode ?? 0,
                SubjectName = subject.Title,
                PaperLastUploadedDate = questionSheets.Where(x => x.IsActive).MaxBy(x => x.Id)?.CreatedOn.ToString("dd-MM-yyyy HH:mm:ss") ?? null
            });
        }
        
        var result = new PCPResponseDTO()
        {
            ClassId = classId,
            PaperTypeId = type,
            PaperType = type == 1 ? "ePCP Final Paper" : "Practice Paper",
            Questions = questions
        };
        
        return result;
    }
    
    public async Task<List<PCPQuestionResponseDTO>> GetUploadedQuestionSheets(int subjectCode, int type)
    {
        var questionSheets =
            await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                x.SubjectId == subjectCode && x.PaperType == type);

        var subject = await _genericRepository.GetByIdAsync<tblSubject>(subjectCode);
        
        return questionSheets.OrderByDescending(x => x.Id).Select(x => new PCPQuestionResponseDTO()
        {
            Id = x.Id,
            Code = subject?.SubjectCode ?? 1,
            Subject = subject?.Title ?? "",
            Type = type == 1 ? "ePCP Final Paper" : "Practice Paper",
            IsArchived = !x.IsActive,
            UploadedDate = x.CreatedOn.ToString("dd-MM-yyyy HH-mm-ss")
        }).ToList();
    }

    public async Task<(string, string)> GetUploadedQuestionSheetById(int questionSheetId)
    {
        var questionSheet = await _genericRepository.GetByIdAsync<tblQuestionPaperSheet>(questionSheetId);

        return questionSheet == null ? ("", "") : (questionSheet.UploadedFileName, questionSheet.UploadedFileUrl);
    }

    public async Task<(bool, string)> IsUploadedSheetValid(PCPQuestionRequestDTO question)
    {
        try
        {
            var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
                x.SubjectCode == question.Code);

            if (subject == null) return (false, "Subject with the respective code could not be found.");
        
            using var workbook = new XLWorkbook(question.QuestionSheet.OpenReadStream());
   
            var worksheet = workbook.Worksheet(1);
        
            var questionsList = worksheet.Rows().Skip(1)
                .Select(row => new 
                {
                    ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                    SubjectCode = row.Cell(2).GetValue<int?>() ?? 0, 
                    IsMandatory = row.Cell(3).GetValue<string?>()?.ToUpper() == "YES", 
                    SequenceNumber = row.Cell(4).GetValue<int?>() ?? 0, 
                    Question = row.Cell(5).GetValue<string?>(),
                    Commons = new List<string>()
                    {
                        row.Cell(6).GetValue<string?>() ?? "",
                        row.Cell(7).GetValue<string?>() ?? "",
                        row.Cell(8).GetValue<string?>() ?? "",
                        row.Cell(9).GetValue<string?>() ?? "",
                    },
                    Language = row.Cell(10).GetValue<string?>() is "Hindi" or "हिंदी" ? 1 : 2,
                    CorrectAnswer = row.Cell(11).GetValue<string?>()?.ToUpper() ?? "A"
                }).ToList();

            var isValid = questionsList.Any(x =>
                x.ClassId == 0 || x.SubjectCode == 0 || x.SequenceNumber == 0 ||
                x.Commons.Any(string.IsNullOrEmpty) || string.IsNullOrEmpty(x.Question));

            if (isValid)
            {
                return (false, "Please do not leave any fields empty.");
            }
            
            foreach (var item in questionsList)
            {
                if (item.ClassId != question.Class)
                    return (false, "Please insert the same value of class for all the columns in the following sheet.");
            
                if (item.SubjectCode != question.Code)
                    return (false, "Please insert the same value of subject code for all the columns in the following sheet.");
            
                if (item.CorrectAnswer != "A" && item.CorrectAnswer != "B" && item.CorrectAnswer != "C" && item.CorrectAnswer != "D")
                    return (false, "Please insert a valid correct option value from the provided options.");
            }
        
            return (true, "Sheet successfully validated.");
        }
        catch (Exception ex)
        {
            return (false, "An exception occured while processing your request, please upload a valid file");
        }
        
    }

    public async Task UploadQuestions(PCPQuestionRequestDTO question)
    {
        var subject = await _genericRepository.GetFirstOrDefaultAsync<tblSubject>(x => 
            x.SubjectCode == question.Code);

        if(subject == null) return;
        
        using var workbook = new XLWorkbook(question.QuestionSheet.OpenReadStream());
   
        var worksheet = workbook.Worksheet(1);
        
        var questionsList = worksheet.Rows().Skip(1)
            .Select(row => new 
            {
                ClassId = row.Cell(1).GetValue<int?>() ?? 0, 
                SubjectId = subject.Id, 
                IsMandatory = row.Cell(3).GetValue<string?>()?.ToUpper() == "YES", 
                SequenceNumber = row.Cell(4).GetValue<int?>() ?? 0, 
                Question = row.Cell(5).GetValue<string?>(),
                Commons = new List<string>()
                {
                    row.Cell(6).GetValue<string?>() ?? "",
                    row.Cell(7).GetValue<string?>() ?? "",
                    row.Cell(8).GetValue<string?>() ?? "",
                    row.Cell(9).GetValue<string?>() ?? "",
                },
                Language = row.Cell(10).GetValue<string?>() is "Hindi" or "हिंदी" ? 1 : 2,
                CorrectAnswer = row.Cell(11).GetValue<string?>()?.ToUpper() ?? "A"
            }).ToList();

        await UploadQuestionsToArchive(subject.Id, question.PaperTypeId);

        foreach (var item in questionsList)
        {
            var questionModel = new tblQuestion()
            {
                Class = item.ClassId,
                SubjectId = subject.Id,
                TopicId = question.PaperTypeId == 1 ? 0 : new Random().Next(1, 500),
                Question = item.Question,
                IsMandatory = item.IsMandatory,
                Sequence = item.SequenceNumber,
                QuestionTypeId = 1,
                PaperType = question.PaperTypeId,
                IsActive = true,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
            };

            var questionId = await _genericRepository.InsertAsync(questionModel);

            var insertedQuestionModel = await _genericRepository.GetByIdAsync<tblQuestion>(questionId);

            if (insertedQuestionModel == null) continue;
            
            insertedQuestionModel.Flag = questionId;
                
            await _genericRepository.UpdateAsync(insertedQuestionModel);

            var correctAnswer = item.CorrectAnswer switch
            {
                "A" => 0,
                "B" => 1,
                "C" => 2,
                "D" => 3,
                _ => 0
            };
            
            for (var i = 0; i < item.Commons.Count; i++)
            {
                var commonModel = new tblCommon()
                {
                    Flag = questionId,
                    Score = 1,
                    LanguageId = item.Language,
                    Value = item.Commons[i],
                    CommonId = i + 1,
                    IsActive = true,
                    CorrectAnswer = correctAnswer == i ? 1 : 0,
                    CreatedBy = 1,
                    CreatedOn = DateTime.Now,
                };
                
                await _genericRepository.InsertAsync(commonModel);
                
            }
        }
    }

    public async Task UploadQuestionsWorksheet(PCPQuestionSheetRequestDTO questionSheet)
    {
        var questionPaperSheet = new tblQuestionPaperSheet()
        {
            SubjectId = questionSheet.SubjectId,
            Class = questionSheet.Class,
            UploadedFileName = questionSheet.UploadedFileName,
            UploadedFileUrl = questionSheet.UploadedFileUrl,
            PaperType = questionSheet.PaperType,
            IsActive = true,
            CreatedBy = 1,
            CreatedOn = DateTime.Now
        };

        await _genericRepository.InsertAsync(questionPaperSheet);
    }

    public async Task ArchiveQuestions(int subjectId, int type)
    {
        await UploadQuestionsToArchive(subjectId, type);
    }
    
    private async Task UploadQuestionsToArchive(int subjectId, int type)
    {
        var questions = await _genericRepository.GetAsync<tblQuestion>(x => 
            x.SubjectId == subjectId && x.PaperType == type);

        var commons = await _genericRepository.GetAsync<tblCommon>(x => 
            questions.Select(z => z.Flag).Contains(x.Flag));

        var questionsList = questions as tblQuestion[] ?? questions.ToArray();

        var questionsArchive = questionsList.Select(x => new tblQuestionsArchive()
        {
            SubjectId = x.SubjectId,
            Class = x.Class,
            Question = x.Question,
            Sequence = x.Sequence,
            TopicId = x.TopicId,
            QuestionTypeId = x.QuestionTypeId,
            Flag = x.Flag,
            IsMandatory = x.IsMandatory,
            PaperType = x.PaperType,
            IsActive = false,
            CreatedOn = x.CreatedOn,
            CreatedBy = x.CreatedBy,
        });

        var commonsList = commons as tblCommon[] ?? commons.ToArray();
        
        var commonsArchive = commonsList.Select(x => new tblCommonsArchive()
        {
            CommonId = x.CommonId,
            Flag = x.Flag,
            Value = x.Value,
            CorrectAnswer = x.CorrectAnswer,
            Score = x.Score,
            LanguageId = x.LanguageId,
            IsActive = false,
            CreatedOn = x.CreatedOn,
            CreatedBy = x.CreatedBy,
        });

        await _genericRepository.AddMultipleEntityAsync(questionsArchive);
        
        await _genericRepository.AddMultipleEntityAsync(commonsArchive);

        await _genericRepository.RemoveMultipleEntityAsync(questionsList);

        await _genericRepository.RemoveMultipleEntityAsync(commonsList);

        var subjectQuestionSheets =
            await _genericRepository.GetAsync<tblQuestionPaperSheet>(x =>
                x.SubjectId == subjectId && x.PaperType == type);

        foreach (var sheet in subjectQuestionSheets)
        {
            sheet.IsActive = false;

            await _genericRepository.UpdateAsync(sheet);
        }
    }
}