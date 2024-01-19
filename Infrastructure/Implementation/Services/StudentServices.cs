using Application.DTOs.Students;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
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
            var findStudentByID = await _genericRepository.GetByIdAsync<Student>(studentId);
            if (findStudentByID != null)
            {
                await _genericRepository.DeleteAsync(findStudentByID);    
            }
        }

        public async Task<List<StudentResponseDTO>> GetAllStudents()
        {
            var studentList = await _genericRepository.GetAsync<Student>(); 
            var result = new List<StudentResponseDTO>();

            foreach (var item in studentList)
            {
                var studentResponse = new StudentResponseDTO()
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
                    SSOID= item.SSOID,  
                    IsEligible = item.IsEligible,
                    AICode = item.AICode    
                };

                result.Add(studentResponse);
            }

            return result; 
        }


        public async Task<Student> GetStudentByID(int studentId)
        {
            var student = await _genericRepository.GetByIdAsync<Student>(studentId);    

            return student;
        }

        public async Task UpdateStudent(StudentResponseDTO studentResponse)
        {
            var existinngStudentDetails = await _genericRepository.GetByIdAsync<Student>(studentResponse.StudentId);

            if (existinngStudentDetails != null)
            {
                existinngStudentDetails.FatherName = studentResponse.FatherName;
                existinngStudentDetails.Name    = studentResponse.Name;
                existinngStudentDetails.AICenterName = studentResponse.AICenterName;    
                existinngStudentDetails.MotherName = studentResponse.MotherName;
                existinngStudentDetails.FatherName = studentResponse.FatherName;
                existinngStudentDetails.AICenterName = studentResponse.AICenterName;
                existinngStudentDetails.AICode = studentResponse.AICode;
                existinngStudentDetails.Course = studentResponse.Course;
                existinngStudentDetails.DateOfBirth = studentResponse.DateOfBirth;
                existinngStudentDetails.Enrollment = studentResponse.Enrollment;
                existinngStudentDetails.Gender = studentResponse.Gender;
                    
                await _genericRepository.UpdateAsync(existinngStudentDetails);  
            }
        }
    }
}
