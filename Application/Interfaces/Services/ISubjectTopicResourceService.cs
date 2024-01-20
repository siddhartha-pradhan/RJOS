using Model.Models;
using Application.DTOs.SubjectTopicResource;

namespace Application.Interfaces.Services;

public interface ISubjectTopicResourceService
{
    Task<List<SubjectTopicResourceResponseDTO>> GetAllSubjectTopicResource();
    
    Task AddSubjectTopicResource(SubjectTopicResourceRequestDTO subjectTopicResourceRequest);
    
    Task UpdateSubjectTopicResource(SubjectTopicResourceResponseDTO subjectTopicResourceResponse);
    
    Task DeleteSubjectTopicResource(int subjectTopicResourceResponseId);
    
    Task<SubjectTopicResourceResponseDTO> GetSubjectTopicResourceById(int subjectTopicResourceResponseId);
}