namespace Application.DTOs.Student;

public class StudentScoreRequestDTO
{
    public int StudentId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public string Score { get; set; } = null!;

    public bool IsEdited { get; set; } = false;

    public bool IsUploaded { get; set; } = true;
}