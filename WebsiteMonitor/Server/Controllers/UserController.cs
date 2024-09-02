using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize(AuthenticationSchemes = "GitHubOAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserGetDto>> GetUser()
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var user = await _userService.GetUserByIDAsync(UserID);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserGetDto>> AddUser(UserPostDto userDto)
        {
            try
            {
                var newUser = await _userService.AddUserAsync(userDto);
                return CreatedAtAction(nameof(AddUser), new { id = newUser.UserID }, newUser);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
