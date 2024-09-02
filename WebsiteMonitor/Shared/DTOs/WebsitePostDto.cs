using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;
public class WebsitePostDto
{
    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The UserID field is required")] 
    public int UserID { get; set; }
}