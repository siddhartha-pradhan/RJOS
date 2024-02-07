using Application.DTOs.NewsAndAlert;
using Application.DTOs.PcpDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPcpDatesService
    {
        Task<List<PcpDatesResponseDTO>> GetAllPcpDates();
        Task InsertPcpDates(PcpDatesRequestDTO pcpDates);
    }
}
