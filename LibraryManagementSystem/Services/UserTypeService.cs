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
        public UserTypeDto CreateUserType(CreateUserTypeDto dto, int createdByUserId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, "User type role");

            var userType = dto.Adapt<UserType>();
            userType.CreatedByUserId = createdByUserId;
            userType.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            userType.IsArchived = false;

            _userTypeRepo.Add(userType);

            return userType.Adapt<UserTypeDto>();
        }

        public List<UserTypeDto> GetAllUserTypes()
        {
            var userTypes = _userTypeRepo.GetAll()
                .Where(u => !u.IsArchived)
                .ToList();

            return userTypes.Adapt<List<UserTypeDto>>();
        }

        public UserTypeDto GetUserTypeById(int id)
        {
            Validate.Positive(id, "Id");

            var userType = _userTypeRepo.GetById(id);

            return userType!.Adapt<UserTypeDto>();
        }

        public UserTypeDto UpdateUserType(UpdateUserTypeDto dto, int userId, int userTypeId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, "User type role");
            Validate.Positive(userTypeId, "User type id");

            var userType = _userTypeRepo.GetById(userTypeId);

            userType.Role = dto.Role;
            userType.LastModifiedByUserId = userId;
            userType.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            _userTypeRepo.Update(userType);

            return userType.Adapt<UserTypeDto>();
        }

        public bool ArchiveUserType(int id, int archivedByUserId)
        {
            var userType = _userTypeRepo.GetById(id);

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
