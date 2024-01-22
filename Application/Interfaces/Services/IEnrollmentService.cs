using Application.DTOs.Enrollment;

namespace Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDTO> GetEnrollmentStatus(int enrollmentId);
    
    Task InsertEnrollments(List<EnrollmentRequestDTO> enrollmentDetails);
}