using Application.DTOs.PcpDate;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services
{
    public class PcpDatesService : IPcpDatesService
    {
        private readonly IGenericRepository _genericRepository;

        public PcpDatesService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<List<PcpDatesResponseDTO>> GetAllPcpDates()
        {
            var pcpDates = await _genericRepository.GetAsync<tblPCPDate>(x => x.IsActive);

            return pcpDates.Select(x => new PcpDatesResponseDTO
            {
                 Id = x.Id,
                 StartDate = x.StartDate,
                 EndDate = x.EndDate,
                 CreatedBy = x.CreatedBy,
                 CreatedOn = x.CreatedOn,
                 IsActive = x.IsActive 
                
            }).ToList();
        }

        public async Task InsertPcpDates(PcpDatesRequestDTO pcpDates)
        {
            var pcpDatesModel = new tblPCPDate()
            {
                StartDate = pcpDates.StartDate,
                EndDate = pcpDates.EndDate,
                IsActive = true,
                CreatedBy = pcpDates.UserId,
                CreatedOn = DateTime.Now,
            };

            await _genericRepository.InsertAsync(pcpDatesModel);
        }
    }
}
