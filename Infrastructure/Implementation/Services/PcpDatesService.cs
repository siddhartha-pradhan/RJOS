using Application.DTOs.NewsAndAlert;
using Application.DTOs.PcpDate;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var pcpDates = await _genericRepository.GetAsync<tblPcpDates>(x => x.IsActive);

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
            var pcpDatesModel = new tblPcpDates()
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
