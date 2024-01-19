using Application.DTOs.Students;
using Model.Models;

namespace Application.Interfaces.Services
{
    public interface IStudentService
    {
        Task<List<StudentResponseDTO>> GetAllStudents();
        Task AddStudent(StudentRequestDTO studentRequest);
        Task UpdateStudent(StudentResponseDTO studentRequest);  
        Task DeleteStudent(int studentId);  
        Task<Student> GetStudentByID(int studentId);  
    }
}
