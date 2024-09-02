using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public class MonitorLogPostDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "The ResponseStatus field is required")] 
    public int ResponseStatus { get; set; }
}
