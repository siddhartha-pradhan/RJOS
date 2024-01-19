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

namespace Data.Implementation.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository _genericRepository;
        public SubjectService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task AddSubjects(SubjectRequestDTO subjectRequestDTO)
        {
            var addSubject = new Subject()
            {
                ClassId = subjectRequestDTO.ClassId,
                Name = subjectRequestDTO.Name,
                LanguageId = subjectRequestDTO.LanguageId,
                Sequence = subjectRequestDTO.Sequence,
                CreatedBy = 1,
            };

            await _genericRepository.InsertAsync(addSubject);
        }

        public async Task DeleteSubject(int subjectId)
        {
            var findSubjectById = await _genericRepository.GetByIdAsync<Subject>(subjectId);
            if (findSubjectById != null)
            {
                await _genericRepository.DeleteAsync(findSubjectById);
            }
        }

        public async Task<List<SubjectResponseDTO>> GetAllSubjects()
        {
            var subjectList = await _genericRepository.GetAsync<Subject>();
            var result = new List<SubjectResponseDTO>();

            foreach (var item in subjectList)
            {
                var subjectResponse = new SubjectResponseDTO()
                {
                    Id = item.Id,
                    ClassId = item.ClassId,
                    LanguageId= item.LanguageId, 
                    Sequence = item.Sequence,
                    Name = item.Name    
                };

                result.Add(subjectResponse);
            }

            return result;
        }

        public async Task<Subject> GetSubjectById(int subjectId)
        {
            var subject = await _genericRepository.GetByIdAsync<Subject>(subjectId);

            return subject;
        }

        public async Task UpdateSubject(SubjectResponseDTO subjectResponseDTO)
        {
            var existingSubjectDetails = await _genericRepository.GetByIdAsync<Subject>(subjectResponseDTO);

            if (existingSubjectDetails != null)
            {
                existingSubjectDetails.Sequence = subjectResponseDTO.Sequence;
                existingSubjectDetails.Name = subjectResponseDTO.Name;
                existingSubjectDetails.ClassId = subjectResponseDTO.ClassId;
                existingSubjectDetails.LanguageId = subjectResponseDTO.LanguageId;

                await _genericRepository.UpdateAsync(existingSubjectDetails);
            }
        }
    }

}