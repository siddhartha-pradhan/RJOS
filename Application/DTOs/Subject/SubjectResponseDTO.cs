namespace Application.DTOs.Subject;

public class SubjectResponseDTO
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? SubjectCode { get; set; }

    public string? TitleInHindi { get; set; }
    
    public int Class { get; set; }
}