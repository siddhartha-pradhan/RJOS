using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Subject;

public class SubjectRequestDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? SubjectCode { get; set; }

    public string? TitleInHindi { get; set; }
}