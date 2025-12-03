using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.DTOs.UserType;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class UserTypeService
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
            Validate.NotEmpty(dto.Role, "User type role");
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
            Validate.Positive(id, "Id");

            var userType = await _userTypeRepo.GetByIdAsync(id);

            return userType!.Adapt<UserTypeDto>();
        }

        public async Task<UserTypeDto> UpdateUserTypeAsync(UpdateUserTypeDto dto, int userId, int userTypeId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, "User type role");
            Validate.Positive(userTypeId, "User type id");
            Validate.Positive(userId, nameof(userId));

            var userType = await _userTypeRepo.GetByIdAsync(userTypeId);

            userType.Role = dto.Role;
            userType.LastModifiedByUserId = userId;
            userType.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            _userTypeRepo.Update(userType);

            return userType.Adapt<UserTypeDto>();
        }

        public async Task<bool> ArchiveUserTypeAsync(int id, int archivedByUserId)
        {
            Validate.Positive(id, "Id");
            Validate.Positive(archivedByUserId, nameof(archivedByUserId));

            var userType = await _userTypeRepo.GetByIdAsync(id);

            if (userType.IsArchived)
                throw new ConflictException($"UserType with id {id} is already archived.");

            userType.IsArchived = true;
            userType.ArchivedByUserId = archivedByUserId;
            userType.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _userTypeRepo.Update(userType);

            return true;
        }
    }
}
