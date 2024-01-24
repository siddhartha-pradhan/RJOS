using Application.DTOs.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Data.Implementation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Implementation.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository _genericRepository;

        public UserService(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<int> GetUserId(UserRequestDTO userRequest)
        {
            var userId = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
            return userId == null ? 0 : userId.Id;
        }

        public async Task<bool> IsUserAuthenticated(UserRequestDTO userRequest)
        {
            var user = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
            
            if (user == null)
            {
                return false;
            }

            if (userRequest.Password == user.Password)
            {
                return true;
            }

            return false;
        }
    }
}
