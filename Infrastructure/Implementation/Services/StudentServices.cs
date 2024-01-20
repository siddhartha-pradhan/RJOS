using Application.DTOs.Students;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Model.Models;

namespace Data.Implementation.Services;

public class StudentServices : IStudentService
{
    private readonly IGenericRepository _genericRepository;
        
    public StudentServices(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task AddStudent(StudentRequestDTO studentRequest)
    {
        var addStudent = new Student()
        {
            AICenterName = studentRequest.AICenterName,
            AICode = studentRequest.AICode,
            Course = studentRequest.Course,
            DateOfBirth = studentRequest.DateOfBirth,
            Enrollment = studentRequest.Enrollment,
            FatherName = studentRequest.FatherName,
            MotherName = studentRequest.MotherName,
            Gender = studentRequest.Gender,
            Name = studentRequest.Name,
            CreatedBy = 1,
            SSOID = studentRequest.SSOID,
            IsEligible = studentRequest.IsEligible,
        };

        await _genericRepository.InsertAsync(addStudent);
    }

    public async Task DeleteStudent(int studentId)
    {
        var studentById = await _genericRepository.GetByIdAsync<Student>(studentId);
            
        if (studentById != null)
        {
            studentById.IsDeleted = true;
                
            await _genericRepository.UpdateAsync(studentById);    
        }
    }

    public async Task<List<StudentResponseDTO>> GetAllStudents()
    {
        var studentList = await _genericRepository.GetAsync<Student>(x => !x.IsDeleted && x.IsActive);

        return studentList.Select(item => new StudentResponseDTO
            {
                Id = item.Id,
                StudentId = item.StudentId,
                AICenterName = item.AICenterName,
                Course = item.Course,
                DateOfBirth = item.DateOfBirth,
                Enrollment = item.Enrollment,
                FatherName = item.FatherName,
                MotherName = item.MotherName,
                Gender = item.Gender,
                Name = item.Name,
                SSOID = item.SSOID,
                IsEligible = item.IsEligible,
                AICode = item.AICode
            })
            .ToList(); 
    }


    public async Task<StudentResponseDTO> GetStudentById(int studentId)
    {
        var student = await _genericRepository.GetByIdAsync<Student>(studentId);

        if (student == null) return new StudentResponseDTO();
        
        var studentResponse = new StudentResponseDTO
        {
            Id = student.Id,
            StudentId = student.StudentId,
            AICenterName = student.AICenterName,
            Course = student.Course,
            DateOfBirth = student.DateOfBirth,
            Enrollment = student.Enrollment,
            FatherName = student.FatherName,   
            MotherName = student.MotherName,   
            Gender = student.Gender,   
            Name = student.Name,   
            SSOID= student.SSOID,  
            IsEligible = student.IsEligible,
            AICode = student.AICode    
        };
                
        return studentResponse;

    }

    public async Task UpdateStudent(StudentResponseDTO studentResponse)
    {
        var existingStudentDetails = await _genericRepository.GetByIdAsync<Student>(studentResponse.Id);

        if (existingStudentDetails != null)
        {
            existingStudentDetails.FatherName = studentResponse.FatherName;
            existingStudentDetails.Name    = studentResponse.Name;
            existingStudentDetails.AICenterName = studentResponse.AICenterName;    
            existingStudentDetails.MotherName = studentResponse.MotherName;
            existingStudentDetails.FatherName = studentResponse.FatherName;
            existingStudentDetails.AICenterName = studentResponse.AICenterName;
            existingStudentDetails.AICode = studentResponse.AICode;
            existingStudentDetails.Course = studentResponse.Course;
            existingStudentDetails.DateOfBirth = studentResponse.DateOfBirth;
            existingStudentDetails.Enrollment = studentResponse.Enrollment;
            existingStudentDetails.Gender = studentResponse.Gender;
                    
            await _genericRepository.UpdateAsync(existingStudentDetails);  
        }
    }
}