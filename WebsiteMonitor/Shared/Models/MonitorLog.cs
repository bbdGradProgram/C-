using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class MonitorLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Explicitly mark the ID as auto-generated
    public int MonitorLogID { get; set; }

    [Required]
    public int WebsiteID { get; set; }

    [Required]
    public DateTime DateChecked { get; set; } = DateTime.UtcNow; //  timestamp set to the time it was created, 
                                                                //ensuring an accurate and consistent timestamp

    [Required]
    public int ResponseStatus { get; set; }
    
    // Navigation properties
    public virtual Website? Website { get; set; }
}
