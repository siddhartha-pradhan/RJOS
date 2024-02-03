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
        
        public async Task<List<SubjectResponseDTO>> GetAllSubjects(int? @class)
        {
            var subjects =
                await _genericRepository.GetAsync<tblSubject>(x => 
                        (!@class.HasValue || x.Class == @class) && x.IsActive);

            return subjects.Select(x => new SubjectResponseDTO
            {
                Id = x.Id,
                Class = x.Class ?? 10,
                SubjectCode = x.SubjectCode,
                Title = x.Title,
                TitleInHindi = x.TitleInHindi
            }).ToList();
        }
    }

