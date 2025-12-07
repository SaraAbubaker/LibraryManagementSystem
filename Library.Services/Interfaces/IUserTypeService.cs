using Library.Shared.DTOs.UserType;

namespace Library.Services.Interfaces
{
    public interface IUserTypeService
    {
        Task<UserTypeDto> CreateUserTypeAsync(CreateUserTypeDto dto, int createdByUserId);
        Task<List<UserTypeDto>> GetAllUserTypesAsync();
        Task<UserTypeDto> GetUserTypeByIdAsync(int id);
        Task<UserTypeDto> UpdateUserTypeAsync(UpdateUserTypeDto dto, int userId, int userTypeId);
        Task<bool> ArchiveUserTypeAsync(int id, int archivedByUserId);
    }
}
