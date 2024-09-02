using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;
public class UserGetDto
{
    public int UserID { get; set; }
    public string Email { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;

    public int GithubID { get; set; }
}