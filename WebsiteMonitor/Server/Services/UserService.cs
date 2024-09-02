using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Models;
using Server.Database;

namespace Server.Services
{
    public interface IUserService
    {
        Task<UserGetDto> GetUserByIDAsync(int id);
        Task<UserGetDto> AddUserAsync(UserPostDto userDto);
        Task ValidateUserDataAsync(UserPostDto userDto);
        Task<int?> GetUserIDFromGithubIDAsync(int githubId);
        Task<User> CreateOrUpdateUserAsync(int githubId, string email, string username);

    }

    public class UserService : IUserService
    {
        private readonly WebsiteMonitorContext _context;
        private readonly IMapper _mapper;

        public UserService(WebsiteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserGetDto> GetUserByIDAsync(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException("User not found.");
            return _mapper.Map<UserGetDto>(user);
        }

        public async Task<UserGetDto> AddUserAsync(UserPostDto userDto)
        {
            await ValidateUserDataAsync(userDto);
            var user = _mapper.Map<User>(userDto);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserGetDto>(user);
        }

        public async Task ValidateUserDataAsync(UserPostDto userDto)
        {
            if (await _context.Users.AnyAsync(u => u.GithubID == userDto.GithubID))
            {
                throw new InvalidOperationException("A user with the same GitHub ID already exists.");
            }
            
            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new InvalidOperationException("A user with the same email already exists.");
            }

            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                throw new InvalidOperationException("A user with the same username already exists.");
            }
        }
 
        public async Task<int?> GetUserIDFromGithubIDAsync(int githubId)
        {
            var user = await _context.Users
                                    .FirstOrDefaultAsync(u => u.GithubID == githubId);
            return user?.UserID;
        }

        public async Task<User> CreateOrUpdateUserAsync(int githubId, string email, string username)
        {
            var user = await _context.Users
                                    .FirstOrDefaultAsync(u => u.GithubID == githubId);

            if (user == null)
            {
                user = new User { GithubID = githubId, Email = email, Username = username };
                _context.Users.Add(user);
            }
            else
            {
                user.Email = email;
                user.Username = username;
            }

            await _context.SaveChangesAsync();
            return user;
        }
    }
}

