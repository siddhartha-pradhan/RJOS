namespace Application.DTOs.Content;

public class EContentResponseDTO
{
    public int Class { get; set; }
    
    public int SubjectId { get; set; }
    
    public int SubjectCode { get; set; }

    public string SubjectName { get; set; }
    
    public List<Contents> ContentsList { get; set; }
}

public class Contents : ContentResponseDTO
{
    public int Sequence { get; set; }

    public int SubjectCode { get; set; }
}