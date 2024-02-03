using Application.DTOs.Tracking;

namespace Application.Interfaces.Services;

public interface IStudentVideoTrackingService
{
    Task<List<StudentVideoTrackingResponseDTO>> GetStudentVideoTrackingByStudentId(int studentId);

    Task InsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTracking);

    Task UpdateStudentVideoTracking(StudentVideoTrackingResponseDTO studentVideoTracking);
}