﻿namespace Application.DTOs.User;

public class UserRequestDTO
{
    public string UserName { get; set; }

    public string Password { get; set; }

    public string Captcha { get; set; }
    
    public string HdUserName { get; set; }
    
    public string HdPassword { get; set; }
    
    public string HdCp { get; set; }
}