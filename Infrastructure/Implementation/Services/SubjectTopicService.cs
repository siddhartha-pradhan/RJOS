using Application.DTOs.Subject;
using Application.DTOs.SubjectTopic;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Model.Models;

namespace Data.Implementation.Services;

public class SubjectTopicService : ISubjectTopicService
{
    private readonly IGenericRepository _genericRepository;
    
    public SubjectTopicService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }


    public async Task AddSubjectTopics(SubjectTopicRequestDTO subjectTopic)
    {
        var addSubjectTopic = new SubjectTopic()
        {
            ClassId = subjectTopic.ClassId,
            Name = subjectTopic.Name,
            SubjectId = subjectTopic.SubjectId,
            Description = subjectTopic.Description,
            CreatedBy = 1,
        };

        await _genericRepository.InsertAsync(addSubjectTopic);
    }

    public async Task DeleteSubjectTopics(int subjectTopicId)
    {
        var subjectTopic = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopicId);
        
        if (subjectTopic != null)
        {
            subjectTopic.IsDeleted = true;
            
            await _genericRepository.UpdateAsync(subjectTopic);
        }
    }

    public async Task<List<SubjectTopicResponseDTO>> GetAllSubjectTopics()
    {
        var subjectTopicList = await _genericRepository.GetAsync<SubjectTopic>(x => x.IsActive && !x.IsDeleted);

        return subjectTopicList.Select(item => new SubjectTopicResponseDTO()
            {
                Id = item.Id,
                Description = item.Description,
                SubjectId = item.SubjectId,
                Name = item.Name,
                ClassId = item.ClassId,
            })
            .ToList();
    }

    public async Task<SubjectTopicResponseDTO> GetSubjectTopicsById(int subjectTopicId)
    {
        var subjectTopic = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopicId);

        var result = new SubjectTopicResponseDTO()
        {
            Id = subjectTopic.Id,
            Description = subjectTopic.Description,
            SubjectId = subjectTopic.SubjectId,
            Name = subjectTopic.Name,
            ClassId = subjectTopic.ClassId,
        };
        
        return result;
    }

    public async Task UpdateSubjectTopics(SubjectTopicResponseDTO subjectTopic)
    {
        var existingSubjectTopicDetails = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopic.Id);

        if (existingSubjectTopicDetails != null)
        {
            existingSubjectTopicDetails.SubjectId = subjectTopic.SubjectId;
            existingSubjectTopicDetails.ClassId = subjectTopic.ClassId;
            existingSubjectTopicDetails.Name = subjectTopic.Name;
            existingSubjectTopicDetails.Description = subjectTopic.Description;

            await _genericRepository.UpdateAsync(existingSubjectTopicDetails);
        }
    }
}