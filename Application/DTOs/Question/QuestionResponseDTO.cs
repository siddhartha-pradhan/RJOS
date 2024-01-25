namespace Application.DTOs.Question;

public class QuestionResponseDTO
{
    public int Id { get; set; }

    public int QuestionTypeId { get; set; }

    public int Class { get; set; }

    public int SubjectId { get; set; }

    public int TopicId { get; set; }

    public bool IsMandatory { get; set; }

    public int? Sequence { get; set; }

    public string? Question { get; set; }

    public int Flag { get; set; }
    
    public List<QuestionCommonResponseDTO> Commons { get; set; }
}

public class QuestionCommonResponseDTO
{
    public int Id { get; set; }

    public int CommonId { get; set; }

    public int Flag { get; set; }

    public string? Value { get; set; }

    public int? LanguageId { get; set; }

    public decimal? Score { get; set; }
}