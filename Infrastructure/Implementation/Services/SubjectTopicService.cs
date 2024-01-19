using Application.DTOs.Subject;
using Application.DTOs.SubjectTopic;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
    public class SubjectTopicService : ISubjectTopicService
    {
        private readonly IGenericRepository _genericRepository;
        public SubjectTopicService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }


        public async Task AddSubjectTopics(SubjectTopicRequestDTO subjectTopicRequestDTO)
        {
            var addSubjectTopic = new SubjectTopic()
            {
                ClassId = subjectTopicRequestDTO.ClassId,
                Name = subjectTopicRequestDTO.Name,
                SubjectId = subjectTopicRequestDTO.SubjectId,
                Description = subjectTopicRequestDTO.Description,
                CreatedBy = 1,
            };

            await _genericRepository.InsertAsync(addSubjectTopic);
        }

        public async Task DeleteSubjectTopics(int subjectTopicId)
        {
            var findSubjectTopicById = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopicId);
            if (findSubjectTopicById != null)
            {
                await _genericRepository.DeleteAsync(findSubjectTopicById);
            }
        }

        public async Task<List<SubjectTopicResponseDTO>> GetAllSubjectTopics()
        {
            var subjectTopicList = await _genericRepository.GetAsync<SubjectTopic>();
            var result = new List<SubjectTopicResponseDTO>();

            foreach (var item in subjectTopicList)
            {
                var subjectTopicResponse = new SubjectTopicResponseDTO()
                {
                    Id = item.Id,
                    Description = item.Description,
                    SubjectId = item.SubjectId,
                    Name = item.Name,
                    ClassId = item.ClassId,
                };

                result.Add(subjectTopicResponse);
            }

            return result;
        }

        public async Task<SubjectTopic> GetSubjectTopicsByID(int subjectTopicId)
        {
            var subjectTopic = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopicId);

            return subjectTopic;
        }

        public async Task UpdateSubjectTopics(SubjectTopicResponseDTO subjectTopicResponseDTO)
        {
            var existingSubjectTopicDetails = await _genericRepository.GetByIdAsync<SubjectTopic>(subjectTopicResponseDTO);

            if (existingSubjectTopicDetails != null)
            {
                existingSubjectTopicDetails.SubjectId = subjectTopicResponseDTO.SubjectId;
                existingSubjectTopicDetails.ClassId = subjectTopicResponseDTO.ClassId;
                existingSubjectTopicDetails.Name = subjectTopicResponseDTO.Name;
                existingSubjectTopicDetails.Description = subjectTopicResponseDTO.Description;


                await _genericRepository.UpdateAsync(existingSubjectTopicDetails);
            }
        }
    }
}
