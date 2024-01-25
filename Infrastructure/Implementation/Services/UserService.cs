using System.Security.Cryptography;
using Application.DTOs.User;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Data.Implementation.Services;

public class UserService : IUserService
{
    private const char SegmentDelimiter = ':';
    private readonly IGenericRepository _genericRepository;

    public UserService(IGenericRepository genericRepository)
    {
        _genericRepository = genericRepository;
    }

    public async Task<int> GetUserId(UserRequestDTO userRequest)
    {
        var userId = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
        return userId?.Id ?? 0;
    }

    public async Task<bool> IsUserAuthenticated(UserRequestDTO userRequest)
    {
        var user = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.UserName == userRequest.UserName);
            
        return user != null && VerifyHash(userRequest.Password, user.Password);
    }

    public async Task<bool> ChangePassword(int userId, string oldPassword, string newPassword)
    {
        var user = await _genericRepository.GetFirstOrDefaultAsync<tblUser>(x => x.Id == userId);
            
        if (user == null) return false;
            
        if (!VerifyHash(oldPassword, user.Password)) return false;
            
        user.Password = HashSecret(newPassword);
            
        await _genericRepository.UpdateAsync(user);
            
        return true;
    }

    private static string HashSecret(string input)
    {
        const int saltSize = 16;
        const int iterations = 100_000;
        const int keySize = 32;
        var algorithm = HashAlgorithmName.SHA256;
        var salt = RandomNumberGenerator.GetBytes(saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(input, salt, iterations, algorithm, keySize);

        var result = string.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            iterations,
            algorithm
        );

        return result;
    }

    private static bool VerifyHash(string input, string hashString)
    {
        var segments = hashString.Split(SegmentDelimiter);
        var hash = Convert.FromHexString(segments[0]);
        var salt = Convert.FromHexString(segments[1]);
        var iterations = int.Parse(segments[2]);
        var algorithm = new HashAlgorithmName(segments[3]);
            
        var inputHash = Rfc2898DeriveBytes.Pbkdf2(
            input,
            salt,
            iterations,
            algorithm,
            hash.Length
        );

        return CryptographicOperations.FixedTimeEquals(inputHash, hash);
    }
}