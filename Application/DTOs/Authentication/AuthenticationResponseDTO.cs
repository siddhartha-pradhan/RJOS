namespace Application.DTOs.Authentication;

public class AuthenticationResponseDTO
{
    public int Id { get; set; }
    
    public string Enrollment { get; set; }
    
    public string Name { get; set; }
    
    public DateTime DateOfBirth { get; set; }
    
    public string SSOID { get; set; }
    
    public string JWT { get; set; }
}