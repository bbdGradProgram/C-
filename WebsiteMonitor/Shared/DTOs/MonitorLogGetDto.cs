namespace Shared.DTOs;

public class MonitorLogGetDto
{
    public int MonitorLogID {get;set;}
    public int WebsiteID { get; set; }
    public DateTime DateChecked { get; set; }
    public int ResponseStatus { get; set; }
}
