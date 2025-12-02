using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
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
            var emailInput = dto.Email.Trim();

            if (_context.Users.Any(u => string.Equals(u.Username, usernameNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already taken.");

            if (_context.Users.Any(u => u.Email == emailInput))
                throw new InvalidOperationException("Email already registered.");

            var user = dto.Adapt<User>();

            user.Username = usernameNormalized;
            user.Email = emailInput;
            user.BorrowRecords = user.BorrowRecords ?? new List<BorrowRecord>();

            user.CreatedByUserId = createdByUserId;
            user.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Users.Add(user);
            _context.SaveChanges();

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

            var input = dto.UsernameOrEmail.Trim();
            var password = dto.Password.Trim();

            var user = _context.Users
                .Include(u => u.BorrowRecords)
                .FirstOrDefault(u =>
                    string.Equals(u.Username, input, StringComparison.OrdinalIgnoreCase) 
                    || u.Email == input
                );

            if (user == null || user.Password != password)
                throw new BadRequestException("Invalid username/email or password.");

            var result = user.Adapt<UserDto>();
            result.BorrowedBooksCount = user.BorrowRecords.Count;

            return result;
        }

        public UserDto GetUserById(int id)
        {
            Validate.Positive(id, "id");

            var user = _context.Users
                .Include(u => u.BorrowRecords)
                .FirstOrDefault(u => u.Id == id);

            Validate.Exists(user, $"User with id {id}");

            var dto = user.Adapt<UserDto>();

            dto.BorrowedBooksCount = user!.BorrowRecords?.Count ?? 0;
            dto.CreatedByUserId = user.CreatedByUserId;
            dto.LastModifiedByUserId = user.LastModifiedByUserId;

            return dto;
        }

        public List<UserDto> GetAllUsers()
        {
            var users = _context.Users
                .Include(u => u.BorrowRecords)
                .ProjectToType<UserDto>() //Mapping
                .ToList();

            foreach (var dto in users)
            {
                dto.BorrowedBooksCount = _context.BorrowRecords
                    .Count(r => r.UserId == dto.Id);
            }

            return users;
        }
    }
}
