using Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<bool> IsUserAuthenticated(UserRequestDTO userRequest);
        Task<int> GetUserId(UserRequestDTO userRequest);   
    }
}
