using Application.DTOs.Students;
using Application.DTOs.Subject;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Data.Implementation.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services;

public class SubjectService : ISubjectService
{
    private readonly IGenericRepository _genericRepository;
    
    public SubjectService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddSubject(SubjectRequestDTO subject)
    {
        var addSubject = new Subject()
        {
            ClassId = subject.ClassId,
            Name = subject.Name,
            LanguageId = subject.LanguageId,
            Sequence = subject.Sequence,
            CreatedBy = 1,
        };

        await _genericRepository.InsertAsync(addSubject);
    }

    public async Task DeleteSubject(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<Subject>(subjectId);
        
        if (subject != null)
        {
            subject.IsDeleted = true;
            
            await _genericRepository.UpdateAsync(subject);
        }
    }

    public async Task<List<SubjectResponseDTO>> GetAllSubjects()
    {
        var subjectList = await _genericRepository.GetAsync<Subject>(x => x.IsActive && !x.IsDeleted);

        return subjectList.Select(item => new SubjectResponseDTO()
            {
                Id = item.Id,
                ClassId = item.ClassId,
                LanguageId = item.LanguageId,
                Sequence = item.Sequence,
                Name = item.Name
            })
            .ToList();
    }

    public async Task<SubjectResponseDTO> GetSubjectById(int subjectId)
    {
        var subject = await _genericRepository.GetByIdAsync<Subject>(subjectId);

        if (subject == null) return new SubjectResponseDTO();
        
        var response = new SubjectResponseDTO
        {
            Id = subject.Id,
            ClassId = subject.ClassId,
            LanguageId = subject.LanguageId,
            Sequence = subject.Sequence,
            Name = subject.Name
        };
            
        return response;

    }

    public async Task UpdateSubject(SubjectResponseDTO subject)
    {
        var existingSubjectDetails = await _genericRepository.GetByIdAsync<Subject>(subject.Id);

        if (existingSubjectDetails != null)
        {
            existingSubjectDetails.Sequence = subject.Sequence;
            existingSubjectDetails.Name = subject.Name;
            existingSubjectDetails.ClassId = subject.ClassId;
            existingSubjectDetails.LanguageId = subject.LanguageId;

            await _genericRepository.UpdateAsync(existingSubjectDetails);
        }
    }
}