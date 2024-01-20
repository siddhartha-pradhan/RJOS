using Application.DTOs.SubjectTopicResource;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Common.Constants;
using Model.Models;

namespace Data.Implementation.Services;

public class SubjectTopicResourceService : ISubjectTopicResourceService
{
    private readonly IGenericRepository _genericRepository;
        
    public SubjectTopicResourceService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddSubjectTopicResource(SubjectTopicResourceRequestDTO subjectTopicResourceRequest)
    {
        var addSubjectTopicResource = new SubjectTopicResource()
        {
            SubjectId = subjectTopicResourceRequest.SubjectId,
            ClassId = subjectTopicResourceRequest.ClassId,
            Title = subjectTopicResourceRequest.Title,
            TopicId = subjectTopicResourceRequest.TopicId,
            ResourceType = (ResourceType)subjectTopicResourceRequest.ResourceType,
            ResourceTypeAttachment = subjectTopicResourceRequest.ResourceTypeAttachment,
            CreatedBy = 1,
        };

        await _genericRepository.InsertAsync(addSubjectTopicResource);
    }

    public async Task DeleteSubjectTopicResource(int subjectTopicResourceResponseId)
    {
        var resource = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponseId);
            
        if (resource != null)
        {
            resource.IsDeleted = true;
                
            await _genericRepository.UpdateAsync(resource);
        }
    }

    public async Task<List<SubjectTopicResourceResponseDTO>> GetAllSubjectTopicResource()
    {
        var subjectTopicResourceList = await _genericRepository.GetAsync<SubjectTopicResource>();

        return subjectTopicResourceList.Select(item => new SubjectTopicResourceResponseDTO()
            {
                Id = item.Id,
                ClassId = item.ClassId,
                Title = item.Title,
                SubjectId = item.SubjectId,
                TopicId = item.TopicId,
                ResourceType = (int)item.ResourceType,
                ResourceTypeAttachment = item.ResourceTypeAttachment,
            })
            .ToList();
    }

    public async Task<SubjectTopicResourceResponseDTO> GetSubjectTopicResourceById(int subjectTopicResourceResponseId)
    {
        var subjectTopicResource = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponseId);

        if (subjectTopicResource == null) return new SubjectTopicResourceResponseDTO();
            
        return new SubjectTopicResourceResponseDTO
        {
            Id = subjectTopicResource.Id,
            ClassId = subjectTopicResource.ClassId,
            Title = subjectTopicResource.Title,
            SubjectId = subjectTopicResource.SubjectId,
            TopicId = subjectTopicResource.TopicId,
            ResourceType = (int)subjectTopicResource.ResourceType,
            ResourceTypeAttachment = subjectTopicResource.ResourceTypeAttachment,
        };
    }

    public async Task UpdateSubjectTopicResource(SubjectTopicResourceResponseDTO subjectTopicResourceResponse)
    {
        var resource = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponse.Id);

        if (resource != null)
        {
            resource.SubjectId = subjectTopicResourceResponse.SubjectId;
            resource.ClassId = subjectTopicResourceResponse.ClassId;
            resource.TopicId = subjectTopicResourceResponse.TopicId;
            resource.SubjectId = subjectTopicResourceResponse.SubjectId;
            resource.Title = subjectTopicResourceResponse.Title;
            resource.ResourceType = (ResourceType)subjectTopicResourceResponse.ResourceType;
            resource.ResourceTypeAttachment = subjectTopicResourceResponse.ResourceTypeAttachment;

            await _genericRepository.UpdateAsync(resource);
        }
    }
}