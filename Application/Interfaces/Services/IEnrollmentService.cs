using Application.DTOs.Enrollment;
using ClosedXML.Excel;

namespace Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDTO> GetEnrollmentStatus(int enrollmentId);
    
    Task InsertEnrollments(List<EnrollmentRequestDTO> enrollmentDetails);
    
    XLWorkbook DownloadEnrollmentSheet();

    List<EnrollmentRequestDTO> ProcessWorksheet(IXLWorksheet worksheet);
}