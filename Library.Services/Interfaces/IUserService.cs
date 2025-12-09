
using Library.Shared.DTOs.User;

namespace Library.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> RegisterUserAsync(RegisterUserDto dto);
        Task<UserDto> LoginUserAsync(LoginDto dto);
        IQueryable<UserDto> GetUserByIdQuery(int id);
        IQueryable<UserDto> GetAllUsersQuery();
        Task<UserDto> ArchiveUserAsync(int id, int performedByUserId);
    }
}
