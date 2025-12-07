using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.UserType;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;

namespace Library.Services.Services
{
    public class UserTypeService : IUserTypeService
    {
        private readonly IGenericRepository<UserType> _userTypeRepo;

        public UserTypeService(IGenericRepository<UserType> userTypeRepo)
        {
            _userTypeRepo = userTypeRepo;
        }

        //CRUD
        public async Task<UserTypeDto> CreateUserTypeAsync(CreateUserTypeDto dto, int createdByUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, nameof(dto.Role));
            Validate.Positive(createdByUserId, nameof(createdByUserId));

            var userType = dto.Adapt<UserType>();
            userType.CreatedByUserId = createdByUserId;
            userType.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            userType.IsArchived = false;

            await _userTypeRepo.AddAsync(userType);

            return userType.Adapt<UserTypeDto>();
        }

        public async Task<List<UserTypeDto>> GetAllUserTypesAsync()
        {
            var userTypes = (await _userTypeRepo.GetAllAsync())
                .Where(u => !u.IsArchived)
                .ToList();

            return userTypes.Adapt<List<UserTypeDto>>();
        }

        public async Task<UserTypeDto> GetUserTypeByIdAsync(int id)
        {
            Validate.Positive(id, nameof(id));

            var userType = await _userTypeRepo.GetByIdAsync(id);

            return userType!.Adapt<UserTypeDto>();
        }

        public async Task<UserTypeDto> UpdateUserTypeAsync(UpdateUserTypeDto dto, int userId, int userTypeId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, nameof(dto.Role));
            Validate.Positive(userTypeId, nameof(userTypeId));
            Validate.Positive(userId, nameof(userId));

            var userType = await _userTypeRepo.GetByIdAsync(userTypeId);

            userType.Role = dto.Role;
            userType.LastModifiedByUserId = userId;
            userType.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userTypeRepo.UpdateAsync(userType);

            return userType.Adapt<UserTypeDto>();
        }

        public async Task<bool> ArchiveUserTypeAsync(int id, int archivedByUserId)
        {
            Validate.Positive(id, nameof(id));
            Validate.Positive(archivedByUserId, nameof(archivedByUserId));

            var userType = await _userTypeRepo.GetByIdAsync(id);

            if (userType.IsArchived)
                throw new ConflictException($"UserType with id {id} is already archived.");

            userType.IsArchived = true;
            userType.ArchivedByUserId = archivedByUserId;
            userType.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userTypeRepo.UpdateAsync(userType);

            return true;
        }
    }
}
