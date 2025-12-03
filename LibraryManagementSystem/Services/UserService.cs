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


        public UserDto RegisterUser(RegisterUserDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.Username, "Username");
            Validate.NotEmpty(dto.Email, "Email");
            Validate.NotEmpty(dto.Password, "Password");

            var usernameNormalized = dto.Username.Trim();
            var emailInput = dto.Email.Trim();

            if (_userRepo.GetAll().Any(u => string.Equals(u.Username, usernameNormalized, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Username already taken.");

            if (_userRepo.GetAll().Any(u => u.Email == emailInput))
                throw new InvalidOperationException("Email already registered.");

            //Normal user
            var userType = _userTypeRepo.GetById(2); 

            var user = dto.Adapt<User>();
            user.Username = usernameNormalized;
            user.Email = emailInput;
            user.UserTypeId = userType.Id;
            user.BorrowRecords = new List<BorrowRecord>();
            user.CreatedByUserId = null;  // No creator for self-registration
            user.CreatedDate = DateOnly.FromDateTime(DateTime.Now);

            _userRepo.Add(user);

            var outDto = user.Adapt<UserDto>();
            outDto.UserTypeId = user.UserTypeId;
            outDto.UserRole = userType.Role;
            outDto.BorrowedBooksCount = 0;
            outDto.CreatedByUserId = user.CreatedByUserId;
            outDto.CreatedDate = user.CreatedDate;

            return outDto;
        }

        public UserDto LoginUser(LoginDto dto)
        {
            Validate.NotNull(dto, nameof(dto));
            Validate.NotEmpty(dto.UsernameOrEmail, "UsernameOrEmail");
            Validate.NotEmpty(dto.Password, "Password");

            var input = dto.UsernameOrEmail.Trim();
            var password = dto.Password.Trim();

            var user = _userRepo.GetAll()
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

            var user = _userRepo.GetById(id);

            var dto = user.Adapt<UserDto>();
            dto.UserRole = user!.UserType!.Role;
            dto.BorrowedBooksCount = user.BorrowRecords?.Count ?? 0;

            return dto;
        }

        public List<UserDto> GetAllUsers()
        {
            var users = _userRepo.GetAll().ToList();

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
