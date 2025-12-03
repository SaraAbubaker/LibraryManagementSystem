using LibraryManagementSystem.Data;
using LibraryManagementSystem.DTOs.User;
using LibraryManagementSystem.Exceptions;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagementSystem.Services
{
    public class UserService
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<UserType> _userTypeRepo;

        public UserService(
            IGenericRepository<User> userRepo,
            IGenericRepository<UserType> userTypeRepo)
        {
            _userRepo = userRepo;
            _userTypeRepo = userTypeRepo;
        }


        public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Username, "Username");
            Validate.NotEmpty(dto.Email, "Email");
            Validate.NotEmpty(dto.Password, "Password");

            var usernameNormalized = dto.Username.Trim();
            var emailInput = dto.Email.Trim();

            var users = await _userRepo.GetAllAsync();

            if (users.Any(u => string.Equals(u.Username, usernameNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already taken.");

            if (users.Any(u => string.Equals(u.Email, emailInput, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already registered.");

            //Normal user
            var userType = (await _userTypeRepo.GetAllAsync())
                   .FirstOrDefault(ut => ut.Role == "Normal");

            if (userType == null)
                throw new InvalidOperationException("Default user type 'Normal' not found in the database.");

            var user = dto.Adapt<User>();
            user.Username = usernameNormalized;
            user.Email = emailInput;
            user.UserTypeId = userType.Id;
            user.BorrowRecords = new List<BorrowRecord>();
            user.CreatedDate = DateOnly.FromDateTime(DateTime.Now);
            user.IsArchived = false;

            await _userRepo.AddAsync(user);

            user.CreatedByUserId = user.Id;
            user.LastModifiedByUserId = user.Id;
            user.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userRepo.UpdateAsync(user);

            var outDto = user.Adapt<UserDto>();
            outDto.UserTypeId = user.UserTypeId;
            outDto.UserRole = userType.Role;
            outDto.BorrowedBooksCount = 0;
            outDto.CreatedByUserId = user.CreatedByUserId;
            outDto.CreatedDate = user.CreatedDate;
            outDto.LastModifiedByUserId = user.LastModifiedByUserId;
            outDto.LastModifiedDate = user.LastModifiedDate;

            return outDto;
        }

        public async Task<UserDto> LoginUserAsync(LoginDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.UsernameOrEmail, "UsernameOrEmail");
            Validate.NotEmpty(dto.Password, "Password");

            var input = dto.UsernameOrEmail.Trim();
            var password = dto.Password.Trim();

            var users = await _userRepo.GetAllAsync();
            var user = users.FirstOrDefault(u =>
                string.Equals(u.Username, input, StringComparison.OrdinalIgnoreCase)
                || string.Equals(u.Email, input, StringComparison.OrdinalIgnoreCase)
            );

            if (user == null || user.Password != password)
                throw new BadRequestException("Invalid username/email or password.");

            var result = user.Adapt<UserDto>();
            result.BorrowedBooksCount = user.BorrowRecords?.Count ?? 0;

            return result;
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            Validate.Positive(id, "id");

            var user = await _userRepo.GetByIdAsync(id);

            var dto = user.Adapt<UserDto>();
            dto.UserRole = user.UserType?.Role ?? "Unknown";
            dto.BorrowedBooksCount = user.BorrowRecords?.Count ?? 0;

            return dto;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = (await _userRepo.GetAllAsync()).ToList();

            var dtos = users.Adapt<List<UserDto>>();

            for (int i = 0; i < dtos.Count; i++)
            {
                dtos[i].BorrowedBooksCount = users[i].BorrowRecords.Count;
                dtos[i].UserRole = users[i].UserType?.Role ?? "Unknown";
            }

            return dtos;
        }
    }
}
