using Library.Domain.Repositories;
using Library.Shared.Helpers;
using Library.Shared.DTOs.User;
using Library.Shared.Exceptions;
using Library.Entities.Models;
using Library.Services.Interfaces;
using Mapster;

namespace Library.Services.Services
{
    public class UserService : IUserService
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
            Validate.NotEmpty(dto.Username, nameof(dto.Username));
            Validate.NotEmpty(dto.Email, nameof(dto.Email));
            Validate.NotEmpty(dto.Password, nameof(dto.Password));

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
            Validate.NotEmpty(dto.UsernameOrEmail, nameof(dto.UsernameOrEmail));
            Validate.NotEmpty(dto.Password, nameof(dto.Password));

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
            Validate.Positive(id, nameof(id));

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

        public async Task<UserDto> ArchiveUserAsync(int id, int performedByUserId)
        {
            Validate.Positive(id, nameof(id));
            Validate.Positive(performedByUserId, nameof(performedByUserId));

             var user = await _userRepo.GetByIdAsync(id);
            
            if (user.IsArchived)
                throw new InvalidOperationException($"User with id {id} is already archived.");

            if (user.BorrowRecords != null && user.BorrowRecords.Any(br => br.ReturnDate == null))
                throw new InvalidOperationException("User has active borrowed books. Return them before deleting.");

            user.IsArchived = true;
            user.ArchivedByUserId = performedByUserId;
            user.ArchivedDate = DateOnly.FromDateTime(DateTime.Now);
            user.LastModifiedByUserId = performedByUserId;
            user.LastModifiedDate = DateOnly.FromDateTime(DateTime.Now);

            await _userRepo.UpdateAsync(user);

            var dto = user.Adapt<UserDto>();
            dto.BorrowedBooksCount = user.BorrowRecords?.Count ?? 0;

            return dto;
        }

    }
}
