namespace Application.DTOs.Authentication;

public class LoginResponseDTO
{
    public bool Status { get; set; }
    
    public StudentDataDTO Data { get; set; }
    
    public object Error { get; set; }
}

public class StudentDataDTO
{
    public StudentInfoDTO Student { get; set; }
}

public class StudentInfoDTO
{
    public int Id { get; set; }
    
    public string Enrollment { get; set; }
    
    public string Name { get; set; }
    
    public DateTime Dob { get; set; }
    
    public string SsoId { get; set; }
}