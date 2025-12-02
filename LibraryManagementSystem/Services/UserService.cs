using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class UserService
    {
        private readonly LibraryContext _context;

        public UserService(LibraryContext context)
        {
            _context = context;
        }

        public UserDto RegisterUser(RegisterUserDto dto, int? createdByUserId = null)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Username, "Username");
            Validate.NotEmpty(dto.Email, "Email");
            Validate.NotEmpty(dto.Password, "Password");

            var usernameNormalized = dto.Username.Trim();
            var emailNormalized = dto.Email.Trim().ToLowerInvariant();

            if (_context.Users.Any(u => string.Equals(u.Username, usernameNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already taken.");

            if (_context.Users.Any(u => string.Equals(u.Email, emailNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already registered.");

            var user = dto.Adapt<User>();

            user.Username = usernameNormalized;
            user.Email = emailNormalized;
            user.Id = _context.Users.Count() + 1;

            user.BorrowRecords = user.BorrowRecords ?? new List<BorrowRecord>();

            user.CreatedByUserId = createdByUserId;
            user.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Users.Add(user);

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
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.UsernameOrEmail, "UsernameOrEmail");
            Validate.NotEmpty(dto.Password, "Password");

            var input = dto.UsernameOrEmail.Trim().ToLower();
            var password = dto.Password.Trim();

            var user = _context.Users.FirstOrDefault(u =>
                u.Username.ToLower() == input ||
                u.Email.ToLower() == input);

            if (user == null || user.Password != password)
                throw new BadRequestException("Invalid username/email or password.");

            var result = user.Adapt<UserDto>();
            result.BorrowedBooksCount = user.BorrowRecords.Count;

            return result;
        }

        public UserDto GetUserById(int id)
        {
            Validate.Positive(id, "id");

            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                throw new NotFoundException($"User with id {id} not found.");

            var dto = user.Adapt<UserDto>();

            dto.BorrowedBooksCount = user.BorrowRecords.Count;
            dto.CreatedByUserId = user.CreatedByUserId;
            dto.LastModifiedByUserId = user.LastModifiedByUserId;

            return dto;
        }

        public List<UserDto> GetAllUsers()
        {
            return _context.Users
                .Select(u => u.Adapt<UserDto>())
                .ToList();
        }
    }
}
