using Application.DTOs.Students;
using Application.DTOs.Subject;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services;

public interface ISubjectService
{
    Task<List<SubjectResponseDTO>> GetAllSubjects();
    
    Task AddSubject(SubjectRequestDTO subject);
    
    Task UpdateSubject(SubjectResponseDTO subject);
    
    Task DeleteSubject(int subjectId);
    
    Task<SubjectResponseDTO> GetSubjectById(int subjectId);
}