namespace Application.DTOs.NewsAndAlert;

public class NewsAndAlertResponseDTO
{
    public int Id { get; set; }

    public string Header { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime ValidTill { get; set; }

    public int IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
}