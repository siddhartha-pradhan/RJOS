using Model.Models;
using Application.DTOs.SubjectTopic;

namespace Application.Interfaces.Services;

public interface ISubjectTopicService
{
    Task<List<SubjectTopicResponseDTO>> GetAllSubjectTopics();
    
    Task AddSubjectTopics(SubjectTopicRequestDTO subjectTopic);
    
    Task UpdateSubjectTopics(SubjectTopicResponseDTO subjectTopic);
    
    Task DeleteSubjectTopics(int subjectTopicId);
    
    Task<SubjectTopicResponseDTO> GetSubjectTopicsById(int subjectTopicId);
}