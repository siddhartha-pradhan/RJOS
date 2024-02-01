﻿using Application.DTOs.Question;

namespace Application.Interfaces.Services;

public interface IQuestionService
{
    Task<QuestionResponseDTO> GetAllQuestions(int? classId, int? subjectId);
}