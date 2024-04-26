using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Authentication;

public class AuthenticationRequestDTO
{
    public string SSOID { get; set; }
    
    public string DateOfBirth { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9-_]{22}:[a-zA-Z0-9-_]{140}$", ErrorMessage = "The registration token is in an invalid format.")]
    public string? DeviceRegistrationToken { get; set; }
}