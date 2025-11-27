using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class UserService
    {
        private readonly LibraryDataStore Store;

        public UserService(LibraryDataStore store)
        {
            Store = store;
        }

        public UserDto RegisterUser(RegisterUserDto dto, int? createdByUserId = null)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var usernameNormalized = dto.Username.Trim();
            var emailNormalized = dto.Email.Trim().ToLowerInvariant();

            if (Store.Users.Any(u => string.Equals(u.Username, usernameNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already taken.");

            if (Store.Users.Any(u => string.Equals(u.Email, emailNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already registered.");

            var user = dto.Adapt<User>();

            user.Username = usernameNormalized;
            user.Email = emailNormalized;
            user.Id = Store.Users.Count + 1;

            user.BorrowRecords = user.BorrowRecords ?? new List<BorrowRecord>();

            user.CreatedByUserId = createdByUserId;
            user.CreatedDate = DateTime.Now;

            Store.Users.Add(user);

            var outDto = user.Adapt<UserDto>();
            outDto.BorrowedBooksCount = user.BorrowRecords?.Count ?? 0;
            outDto.CreatedByUserId = user.CreatedByUserId;
            outDto.CreatedDate = user.CreatedDate;
            outDto.LastModifiedByUserId = user.LastModifiedByUserId;
            outDto.LastModifiedDate = user.LastModifiedDate;

            return outDto;
        }

        public UserDto LoginUser(LoginDto dto)
        {
            var input = dto.UsernameOrEmail.Trim().ToLower();
            var password = dto.Password.Trim();

            var user = Store.Users.FirstOrDefault(u =>
                u.Username.ToLower() == input ||
                u.Email.ToLower() == input);

            if (user == null)
                throw new Exception("Invalid username/email or password.");

            if (user.Password != password)
                throw new Exception("Invalid username/email or password.");

            var result = user.Adapt<UserDto>();
            result.BorrowedBooksCount = user.BorrowRecords.Count;

            return result;
        }

        public UserDto GetUserById(int id)
        {
            var user = Store.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                throw new KeyNotFoundException($"User with id {id} not found.");

            var dto = user.Adapt<UserDto>();

            dto.BorrowedBooksCount = user.BorrowRecords.Count;
            dto.CreatedByUserId = user.CreatedByUserId;
            dto.LastModifiedByUserId = user.LastModifiedByUserId;

            return dto;
        }


    }
}
