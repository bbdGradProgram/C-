using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Server.Services;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize(AuthenticationSchemes = "GitHubOAuth")]
    [Route("api/[controller]")]
    [ApiController]
    public class WebsitesController : ControllerBase
    {
        private readonly IWebsiteService _websiteService;

        public WebsitesController(IWebsiteService websiteService)
        {
            _websiteService = websiteService;
        }

        [HttpGet]
        public async Task<ActionResult<WebsiteGetDto>> GetAllWebsites()
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var websites = await _websiteService.GetAllWebsitesAsync(UserID);
            return Ok(websites);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WebsiteGetDto>> GetWebsite(int id)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try{
                var website = await _websiteService.GetWebsiteByIdAsync(UserID, id);
                 return Ok(website);
            }
            catch(KeyNotFoundException ex){
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<WebsiteGetDto>> AddWebsite(WebsitePostDto websiteDto)
        {
            var UserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            try
            {
                websiteDto.UserID = UserID;
                var newWebsite = await _websiteService.AddWebsiteAsync(websiteDto);
                return CreatedAtAction(nameof(GetWebsite), new { id = newWebsite.WebsiteID }, newWebsite);
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
        [HttpDelete("{id}")]
        public async Task<ActionResult<WebsiteGetDto>> RemoveWebsite(int id)
        {
            try
            {
                await _websiteService.RemoveWebsiteAsync(id);
                return NoContent();
            }
            catch(KeyNotFoundException ex){
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
