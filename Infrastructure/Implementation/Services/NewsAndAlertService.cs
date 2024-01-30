using Application.DTOs.NewsAndAlert;
using Application.DTOs.Notification;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
    public class NewsAndAlertService : INewsAndAlertService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository _genericRepository;

        public NewsAndAlertService(IConfiguration configuration, IGenericRepository genericRepository)
        {
            _configuration = configuration;
            _genericRepository = genericRepository;
        }

        public async Task<List<NewsAndAlertResponseDTO>> GetAllNewsAndAlert()
        {
            var newsAndAlert = await _genericRepository.GetAsync<tblNewsAndAlert>(x => x.IsActive);

            return newsAndAlert.Select(x => new NewsAndAlertResponseDTO
            {
                Id = x.Id,
                Header = x.Header,
                Description = x.Description,
                ValidTill = x.ValidTill,
                IsActive = x.IsActive,  
                CreatedOn = x.CreatedOn
            }).ToList();
        }

        public async Task<List<NewsAndAlertResponseDTO>> GetAllValidNewsAndAlert()
        {
            var newsAndAlert = await _genericRepository.GetAsync<tblNewsAndAlert>(x =>
            x.IsActive && x.ValidTill >= DateTime.Now);

            return newsAndAlert.Select(x => new NewsAndAlertResponseDTO
            {
                Id = x.Id,
                Header = x.Header,
                Description= x.Description,
                ValidTill= x.ValidTill,
                IsActive = x.IsActive,
                CreatedOn = x.CreatedOn,
            }).ToList();
        }

        public async Task<NewsAndAlertResponseDTO> GetNewsAndAlertById(int newsAndAlertId)
        {
            var newsAndAlert = await _genericRepository.GetByIdAsync<tblNewsAndAlert>(newsAndAlertId);

            if (newsAndAlert != null)
            {
                return new NewsAndAlertResponseDTO()
                {
                    Id = newsAndAlert.Id,
                    Header = newsAndAlert.Header,
                    Description = newsAndAlert.Description,
                    ValidTill = newsAndAlert.ValidTill,
                    //IsActive= newsAndAlert.IsActive,
                    //CreatedOn = newsAndAlert.CreatedOn
                };
            }

            return new NewsAndAlertResponseDTO();
        }

        public async Task InsertNewsAndAlert(NewsAndAlerRequestDTO newsAndAlert)
        {
            var newsAndAlertModel = new tblNewsAndAlert()
            {
                Header = newsAndAlert.Header,
                Description = newsAndAlert.Description,
                ValidTill = newsAndAlert.ValidTill,
                IsActive = true,
                CreatedBy = newsAndAlert.UserId,
                CreatedOn = DateTime.Now,
            };

            await _genericRepository.InsertAsync(newsAndAlertModel);
        }

        public async Task UpdateNewsAndAlert(NewsAndAlerRequestDTO newsAndAlert)
        {
            var newsAndAlertModel = await _genericRepository.GetByIdAsync<tblNewsAndAlert>(newsAndAlert.Id);

            if (newsAndAlertModel != null)
            {
                newsAndAlertModel.Header = newsAndAlert.Header;
                newsAndAlertModel.Description = newsAndAlert.Description;
                newsAndAlertModel.ValidTill = newsAndAlert.ValidTill;

                newsAndAlertModel.LastUpdatedBy = newsAndAlert.UserId;
                newsAndAlertModel.LastUpdatedOn = DateTime.Now;

                await _genericRepository.UpdateAsync(newsAndAlertModel);
            }
        }
    }
}
