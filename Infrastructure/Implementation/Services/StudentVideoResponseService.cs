using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Application.DTOs.Tracking;

namespace Data.Implementation.Services;

public class StudentVideoResponseService : IStudentVideoTrackingService
{
    private readonly IGenericRepository _genericRepository;

    public StudentVideoResponseService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<List<StudentVideoTrackingResponseDTO>> GetStudentVideoTrackingByStudentId(int studentId)
    {
        var result = await _genericRepository.GetAsync<tblStudentVideoTracking>(x => x.StudentId == studentId && x.IsActive);

        return result.Select(x => new StudentVideoTrackingResponseDTO()
        {
            Id = x.Id,
            SubjectId = x.SubjectId,
            VideoId = x.VideoId,
            StudentId = x.StudentId,
            Class = x.Class,
            IsCompleted = x.IsCompleted ? 1 : 0,
            PlayTimeInSeconds = x.PlayTimeInSeconds,
            PercentageCompleted = x.PercentageCompleted,
            VideoDurationInSeconds = x.VideoDurationInSeconds
        }).ToList();
    }

    public async Task InsertStudentVideoTracking(StudentVideoTrackingRequestDTO studentVideoTracking)
    {
        var studentVideoTrackingModel = new tblStudentVideoTracking()
        {
            SubjectId = studentVideoTracking.SubjectId,
            VideoId = studentVideoTracking.VideoId,
            StudentId = studentVideoTracking.StudentId,
            Class = studentVideoTracking.Class,
            IsCompleted = false,
            PlayTimeInSeconds = studentVideoTracking.PlayTimeInSeconds,
            PercentageCompleted = studentVideoTracking.PercentageCompleted,
            VideoDurationInSeconds= studentVideoTracking.VideoDurationInSeconds,
            IsActive = true,
            CreatedBy = 1,
            CreatedOn = DateTime.Now, 
        };

        await _genericRepository.InsertAsync(studentVideoTrackingModel);
    }

    public async Task UpdateStudentVideoTracking(StudentVideoTrackingResponseDTO studentVideoTrackingResponse)
    {
        var studentVideoTrackingModel = await _genericRepository.GetByIdAsync<tblStudentVideoTracking>(studentVideoTrackingResponse.Id);

        if (studentVideoTrackingModel != null)
        {
            studentVideoTrackingModel.SubjectId = studentVideoTrackingResponse.SubjectId;
            studentVideoTrackingModel.VideoId = studentVideoTrackingResponse.VideoId;
            studentVideoTrackingModel.StudentId = studentVideoTrackingResponse.StudentId;
            studentVideoTrackingModel.Class = studentVideoTrackingResponse.Class;
            studentVideoTrackingModel.PlayTimeInSeconds = studentVideoTrackingResponse.PlayTimeInSeconds;
            studentVideoTrackingModel.PercentageCompleted = studentVideoTrackingResponse.PercentageCompleted;
            studentVideoTrackingModel.VideoDurationInSeconds = studentVideoTrackingResponse.VideoDurationInSeconds;
            studentVideoTrackingModel.IsCompleted = studentVideoTrackingModel.PlayTimeInSeconds == studentVideoTrackingModel.VideoDurationInSeconds;

            await _genericRepository.UpdateAsync(studentVideoTrackingModel);
        }
    }
}