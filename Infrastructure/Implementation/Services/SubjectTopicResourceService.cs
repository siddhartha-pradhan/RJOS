using Application.DTOs.Subject;
using Application.DTOs.SubjectTopicResource;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
    public class SubjectTopicResourceService : ISubjectTopicResourceService
    {
        private readonly IGenericRepository _genericRepository;
        public SubjectTopicResourceService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task AddSubjectTopicResource(SubjectTopicResourceRequestDTO subjectTopicResourceRequest)
        {
            int intValue = 2;
            ResourceType enumvalue = (ResourceType)intValue;
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
            var findSubjectTopicResourceById = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponseId);
            if (findSubjectTopicResourceById != null)
            {
                await _genericRepository.DeleteAsync(findSubjectTopicResourceById);
            }
        }

        public async Task<List<SubjectTopicResourceResponseDTO>> GetAllSubjectTopicResource()
        {
            var subjectTopicResourceList = await _genericRepository.GetAsync<SubjectTopicResource>();
            var result = new List<SubjectTopicResourceResponseDTO>();

            foreach (var item in subjectTopicResourceList)
            {
                var subjectTopicResourceResponse = new SubjectTopicResourceResponseDTO()
                {
                    Id = item.Id,
                    ClassId = item.ClassId,
                    Title = item.Title,
                    SubjectId = item.SubjectId,
                    TopicId = item.TopicId,
                    ResourceType = (int)item.ResourceType,
                    ResourceTypeAttachment = item.ResourceTypeAttachment,
                };

                result.Add(subjectTopicResourceResponse);
            }

            return result;
        }

        public async Task<SubjectTopicResource> GetSubjectTopicResourceById(int subjectTopicResourceResponseId)
        {
            var subjectTopicResource = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponseId);

            return subjectTopicResource;
        }

        public async Task UpdateSubjectTopicResource(SubjectTopicResourceResponseDTO subjectTopicResourceResponse)
        {
            var existingSubjectTopicResourceDetails = await _genericRepository.GetByIdAsync<SubjectTopicResource>(subjectTopicResourceResponse);

            if (existingSubjectTopicResourceDetails != null)
            {
                existingSubjectTopicResourceDetails.SubjectId = subjectTopicResourceResponse.SubjectId;
                existingSubjectTopicResourceDetails.ClassId = subjectTopicResourceResponse.ClassId;
                existingSubjectTopicResourceDetails.TopicId = subjectTopicResourceResponse.TopicId;
                existingSubjectTopicResourceDetails.SubjectId = subjectTopicResourceResponse.SubjectId;
                existingSubjectTopicResourceDetails.ResourceType = (ResourceType)subjectTopicResourceResponse.ResourceType;
                existingSubjectTopicResourceDetails.ResourceTypeAttachment = subjectTopicResourceResponse.ResourceTypeAttachment;

                await _genericRepository.UpdateAsync(existingSubjectTopicResourceDetails);
            }
        }
    }
}
