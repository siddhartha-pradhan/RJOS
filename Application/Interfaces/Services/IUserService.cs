using Application.DTOs.User;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<bool> IsUserAuthenticated(UserRequestDTO userRequest);
    
    Task<int> GetUserId(UserRequestDTO userRequest);   
    
    Task<bool> ChangePassword(int userId, string oldPassword, string newPassword);
}