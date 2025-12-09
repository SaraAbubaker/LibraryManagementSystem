using Library.Shared.DTOs.UserType;

namespace Library.Services.Interfaces
{
    public interface IUserTypeService
    {
        Task<UserTypeDto> CreateUserTypeAsync(CreateUserTypeDto dto, int createdByUserId);
        IQueryable<UserTypeDto> GetAllUserTypesQuery();
        IQueryable<UserTypeDto> GetUserTypeByIdQuery(int id);
        Task<UserTypeDto> UpdateUserTypeAsync(UpdateUserTypeDto dto, int userId, int userTypeId);
        Task<bool> ArchiveUserTypeAsync(int id, int archivedByUserId);
    }
}
