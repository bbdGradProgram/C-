using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Explicitly mark the ID as auto-generated
    public int UserID { get; set; }

    [Required]
    public int GithubID { get; set; }

    [Required]
    [StringLength(120)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [StringLength(120)]
    public string Username { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual List<Website> Websites { get; set; } = [];
}