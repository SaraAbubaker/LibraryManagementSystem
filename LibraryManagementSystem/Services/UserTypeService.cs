using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.DTOs.UserType;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Services
{
    public class UserTypeService
    {
        private readonly LibraryContext _context;

        public UserTypeService(LibraryContext context)
        {
            _context = context;
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

            _context.UserTypes.Add(userType);
            _context.SaveChanges();

            return userType.Adapt<UserTypeDto>();
        }

        public List<UserTypeDto> GetAllUserTypes()
        {
            var userTypes = _context.UserTypes
                .Where(u => !u.IsArchived)
                .ToList();

            return userTypes.Adapt<List<UserTypeDto>>();
        }

        public UserTypeDto GetUserTypeById(int id)
        {
            Validate.Positive(id, "Id");

            var userType = _context.UserTypes.FirstOrDefault(u => u.Id == id);
            Validate.Exists(userType, $"UserType with id {id}");

            return userType!.Adapt<UserTypeDto>();
        }

        public UserTypeDto UpdateUserType(UpdateUserTypeDto dto, int userId, int userTypeId)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Role, "User type role");
            Validate.Positive(userTypeId, "User type id");

            var userType = _context.UserTypes.FirstOrDefault(u => u.Id == userTypeId);
            Validate.Exists(userType, $"UserType with id {userTypeId}");

            userType!.Role = dto.Role;
            userType.LastModifiedByUserId = userId;
            userType.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();

            return userType.Adapt<UserTypeDto>();
        }

        public bool ArchiveUserType(int id, int? archivedByUserId = null)
        {
            Validate.Positive(id, "Id");

            var userType = _context.UserTypes.FirstOrDefault(u => u.Id == id);
            Validate.Exists(userType, $"UserType with id {id}");

            if (userType!.IsArchived)
                throw new ConflictException($"UserType with id {id} is already archived.");

            userType.IsArchived = true;
            userType.ArchivedByUserId = archivedByUserId;
            userType.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.SaveChanges();

            return true;
        }
    }
}
