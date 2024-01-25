namespace Application.DTOs.Student;

public class StudentResponsesResponseDTO
{
    public int Id { get; set; }
    
    public string? GUID { get; set; }

    public string? QuizGUID { get; set; }

    public int StudentId { get; set; }

    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public bool IsEdited { get; set; } = false;

    public bool IsUploaded { get; set; } = true;
}