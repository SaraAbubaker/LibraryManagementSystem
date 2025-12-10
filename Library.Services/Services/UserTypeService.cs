using Library.Domain.Repositories;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Library.Shared.DTOs.UserType;
using Library.Shared.Exceptions;
using Library.Shared.Helpers;
using Mapster;
using Microsoft.EntityFrameworkCore;

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
            Validate.ValidateModel(dto);
            Validate.Positive(createdByUserId, nameof(createdByUserId));

            var userType = dto.Adapt<UserType>();
            userType.CreatedByUserId = createdByUserId;
            userType.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            userType.IsArchived = false;

            await _userTypeRepo.AddAsync(userType);
            await _userTypeRepo.CommitAsync();

            return userType.Adapt<UserTypeDto>();
        }

        public IQueryable<UserTypeDto> GetAllUserTypesQuery()
        {
            return _userTypeRepo.GetAll()
                .AsNoTracking()
                .Select(ut => new UserTypeDto
                {
                    Id = ut.Id,
                    Role = ut.Role
                });
        }

        public IQueryable<UserTypeDto> GetUserTypeByIdQuery(int id)
        {
            Validate.Positive(id, nameof(id));

            return _userTypeRepo.GetAll()
                .AsNoTracking()
                .Where(ut => ut.Id == id)
                .Select(ut => new UserTypeDto
                {
                    Id = ut.Id,
                    Role = ut.Role
                });
        }

        public async Task<UserTypeDto> UpdateUserTypeAsync(UpdateUserTypeDto dto, int userId, int userTypeId)
        {
            Validate.ValidateModel(dto);
            Validate.Positive(userTypeId, nameof(userTypeId));
            Validate.Positive(userId, nameof(userId));

            var userType = Validate.Exists(
                await _userTypeRepo.GetById(userTypeId).FirstOrDefaultAsync(),
                userTypeId
            );

            userType.Role = dto.Role;
            userType.LastModifiedByUserId = userId;
            userType.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userTypeRepo.UpdateAsync(userType);
            await _userTypeRepo.CommitAsync();

            return userType.Adapt<UserTypeDto>();
        }

        public async Task<bool> ArchiveUserTypeAsync(int id, int archivedByUserId)
        {
            Validate.Positive(id, nameof(id));
            Validate.Positive(archivedByUserId, nameof(archivedByUserId));

            var userType = Validate.Exists(
                await _userTypeRepo.GetById(id).FirstOrDefaultAsync(),
                id
            );

            userType.IsArchived = true;
            userType.ArchivedByUserId = archivedByUserId;
            userType.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userTypeRepo.UpdateAsync(userType);
            await _userTypeRepo.CommitAsync();

            return true;
        }
    }
}
