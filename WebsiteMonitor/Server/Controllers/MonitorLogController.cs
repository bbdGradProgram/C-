using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize(AuthenticationSchemes = "GitHubOAuth")]
    [Route("api/Websites/")]
    [ApiController]
    public class MonitorLogsController : ControllerBase
    {
        private readonly IMonitorLogService _monitorLogService;

        public MonitorLogsController(IMonitorLogService monitorLogService)
        {
            _monitorLogService = monitorLogService;
        }

        [HttpGet("{WebsiteID}/monitorlogs")]
        public async Task<ActionResult<MonitorLogGetDto>> GetWebsiteLogs(int WebsiteID)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                var logs = await _monitorLogService.GetLogsByWebsiteIDAsync(UserID, WebsiteID);
                return Ok(logs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{WebsiteID}/monitorlogs")]
        public async Task<ActionResult<MonitorLogGetDto>> AddWebsiteLog(int WebsiteID, MonitorLogPostDto MonitorLogDto)
        {
            try
            {
                var logs = await _monitorLogService.AddWebsiteLogAsync(WebsiteID, MonitorLogDto);
                return Ok(logs);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
