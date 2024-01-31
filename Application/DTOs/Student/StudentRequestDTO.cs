namespace Application.DTOs.Student;

public class StudentRequestDTO
{
    public List<StudentResponseRequestDTO> StudentResponse { get; set; }

    public StudentScoreRequestDTO StudentScore { get; set; }
}

public class StudentResponseRequestDTO
{
    public string? GUID { get; set; }

    public string? QuizGUID { get; set; }

    public int StudentId { get; set; }

    public int QuestionId { get; set; }

    public string? QuestionValue { get; set; }

    public bool IsEdited { get; set; } = false;

    public bool IsUploaded { get; set; } = true;
    
    public int CreatedBy { get; set; }
}

public class StudentScoreRequestDTO
{
    public string GUID { get; set; }
    
    public int StudentId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public string Score { get; set; } = null!;

    public bool IsEdited { get; set; } = false;

    public bool IsUploaded { get; set; } = true;
    
    public int CreatedBy { get; set; }
}