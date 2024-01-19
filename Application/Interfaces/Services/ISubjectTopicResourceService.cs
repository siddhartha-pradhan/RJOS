using Application.DTOs.Subject;
using Application.DTOs.SubjectTopicResource;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ISubjectTopicResourceService
    {
        Task<List<SubjectTopicResourceResponseDTO>> GetAllSubjectTopicResource();
        Task AddSubjectTopicResource(SubjectTopicResourceRequestDTO subjectTopicResourceRequest);
        Task UpdateSubjectTopicResource(SubjectTopicResourceResponseDTO subjectTopicResourceResponse);
        Task DeleteSubjectTopicResource(int subjectTopicResourceResponseId);
        Task<SubjectTopicResource> GetSubjectTopicResourceById(int subjectTopicResourceResponseId);
    }
}
