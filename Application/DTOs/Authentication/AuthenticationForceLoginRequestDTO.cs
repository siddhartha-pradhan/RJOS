namespace Application.DTOs.Authentication;

public class AuthenticationForceLoginRequestDTO
{
    public string SSOID { get; set; }
    
    public string DateOfBirth { get; set; }
    
    public string? DeviceRegistrationToken { get; set; }
    
    public int IsForceLogIn { get; set; }
}