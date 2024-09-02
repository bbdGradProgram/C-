namespace Shared.DTOs;
public class WebsiteGetDto
{
    public WebsiteGetDto(int websiteID, string url, int userID)
    {
        WebsiteID = websiteID;
        Url = url;
        UserID = userID;
    }

    public int WebsiteID { get; set; }
    public string Url { get; set; } = string.Empty;
    public int UserID { get; set; }
}