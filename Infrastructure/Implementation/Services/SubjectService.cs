using Application.DTOs.Subject;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;


namespace Data.Implementation.Services;

    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository _genericRepository;

        public SubjectService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<List<SubjectResponseDTO>> GetAllSubjects()
        {
            var subjects = await _genericRepository.GetAsync<tblSubject>(x => x.IsActive);

            return subjects.Select(x => new SubjectResponseDTO
            {
                Id = x.Id,
                Title = x.Title,
                SubjectCode = x.SubjectCode,
                TitleInHindi = x.TitleInHindi, 
            }).ToList();
        }
    }

