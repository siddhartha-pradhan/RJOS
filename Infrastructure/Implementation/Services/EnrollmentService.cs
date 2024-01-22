using Application.DTOs.Enrollment;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IGenericRepository _repository;

    public EnrollmentService(IGenericRepository repository) 
    {
        _repository = repository;
    }

    public async Task<EnrollmentResponseDTO> GetEnrollmentStatus(int enrollmentId)
    {
        var enrollment = await _repository.GetFirstOrDefaultAsync<tblEnrollmentStatus>(x => x.EnrollmentId == enrollmentId);
        
        if (enrollment != null)
        {
            return new EnrollmentResponseDTO()
            {
                EnrollmentId = enrollment.EnrollmentId,
                Status = enrollment.Status,
                Reason = enrollment.Reason,
            };
        }

        return new EnrollmentResponseDTO();
    }

    public async Task InsertEnrollments(List<EnrollmentRequestDTO> enrollmentDetails)
    {
        var enrollments = enrollmentDetails.Select(enrollment => new tblEnrollmentStatus
            {
                EnrollmentId = enrollment.EnrollmentId,
                Status = enrollment.Status,
                Reason = enrollment.Reason,
                CreatedBy = 1,
                CreatedOn = DateTime.Now,
                IsActive = true,
            })
            .ToList();
        
        await _repository.AddMultipleEntityAsync(enrollments);
    }
}