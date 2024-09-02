using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;
public class Website
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Explicitly mark the ID as auto-generated
    public int WebsiteID { get; set; }
    
    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public int UserID { get; set; }

    // Navigation properties
    public virtual User? User { get; set; }
    public virtual List<MonitorLog> MonitorLogs { get; set; } = [];
}