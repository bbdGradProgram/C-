using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;
public class UserPostDto
{

    [Required]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(120)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The GithubID field is required")] 
    public int GithubID { get; set; }
}