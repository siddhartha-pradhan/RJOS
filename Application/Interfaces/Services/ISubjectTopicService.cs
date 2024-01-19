using Application.DTOs.Subject;
using Application.DTOs.SubjectTopic;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ISubjectTopicService
    {
        Task<List<SubjectTopicResponseDTO>> GetAllSubjectTopics();
        Task AddSubjectTopics(SubjectTopicRequestDTO subjectTopicRequestDTO);
        Task UpdateSubjectTopics(SubjectTopicResponseDTO subjectTopicResponseDTO);
        Task DeleteSubjectTopics(int subjectTopicId);
        Task<SubjectTopic> GetSubjectTopicsByID(int subjectTopicId);
    }
}
