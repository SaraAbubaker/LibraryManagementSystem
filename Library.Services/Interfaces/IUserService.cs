
using Library.Shared.DTOs.User;

namespace Library.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
        Task<UserDto> LoginUserAsync(LoginDto dto);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<List<UserDto>> GetAllUsersAsync();
        Task<UserDto> ArchiveUserAsync(int id, int performedByUserId);
    }
}
