using Application.DTOs.NewsAndAlert;
using Application.DTOs.StudentVideoTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IStudentVideoTrackingService
    {
        Task<List<StudentVideoTrackingResponseDTO>> GetStudentVideoTrackingByStudentId(int studentId);

        Task InsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTracking);

        Task UpdateStudentVideoTracking(StudentVideoTrackingResponseDTO studentVideoTracking);
    }
}
