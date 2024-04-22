﻿using Application.DTOs.Content;
using Application.DTOs.Dropdown;
using Application.DTOs.Subject;

namespace Application.Interfaces.Services;

public interface IContentService
{
    Task<List<ContentResponseDTO>> GetAllContents(int? classId, int? subjectId);

    Task<EContentResponseDTO> GetAllContents(EContentRequestDTO content);

    Task<(bool, string)> UpsertContents(Contents content);

    Task<(bool, bool)> UpdateContentStatus(int contentId);

    Task<List<SelectListModel>> GetSubjectsByClass(int classId);

    Task<SubjectResponseDTO> GetSubjectById(int subjectId);

    Task<Contents> GetContentById(int contentId);
}