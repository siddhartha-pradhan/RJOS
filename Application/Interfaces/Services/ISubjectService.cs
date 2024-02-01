using Application.DTOs.Notification;
using Application.DTOs.Student;
using Application.DTOs.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<List<SubjectResponseDTO>> GetAllSubjects();
    }
}
