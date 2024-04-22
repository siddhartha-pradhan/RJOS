namespace Application.DTOs.EContentBooks;

public class EContentBookResponseDTO
{
    public int Id { get; set; }
    
    public int Code { get; set; }

    public int Class { get; set; }
    
    public string NameOfBook { get; set; } = string.Empty;
    
    public string SubjectName { get; set; } = string.Empty;

    public string Volume { get; set; } = string.Empty;
    
    public string FileName { get; set; }
}