namespace Application.DTOs.Authentication;

public class AuthenticationRequestDTO
{
    public string SSOID { get; set; }
    
    public string DateOfBirth { get; set; }
    
    public string? DeviceRegistrationToken { get; set; }
}