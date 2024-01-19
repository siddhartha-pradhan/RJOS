using Model.Base;
using Common.Constants;

namespace Model.Models;

public class Student : BaseEntity<int>
{
    public Guid StudentId { get; set; }
    
    public string? AICode { get; set; } = default!;
    
    public string Enrollment { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    
    public DateTime DateOfBirth { get; set; }
    
    public string SSOID { get; set; } = default!;
    
    public string? FatherName { get; set; } = default!;
    
    public string? MotherName { get; set; } = default!;
    
    public Gender Gender { get; set; }
    
    public int Course { get; set; }

    public bool IsEligible { get; set; } = false;

    public string? AICenterName { get; set; } = default!;
}