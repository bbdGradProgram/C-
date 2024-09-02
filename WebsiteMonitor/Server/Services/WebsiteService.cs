using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Models;
using Server.Database;

namespace Server.Services
{
    public interface IWebsiteService
    {
        Task<IEnumerable<WebsiteGetDto>> GetAllWebsitesAsync(int UserID);
        Task<WebsiteGetDto> GetWebsiteByIdAsync(int UserID,int id);
        Task<WebsiteGetDto> AddWebsiteAsync(WebsitePostDto websiteDto);
        Task RemoveWebsiteAsync(int id);
    }

    public class WebsiteService : IWebsiteService
    {
        private readonly WebsiteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly IMonitorService _monitorService;

        public WebsiteService(WebsiteMonitorContext context, IMapper mapper, IMonitorService monitorService)
        {
            _context = context;
            _mapper = mapper;
            _monitorService = monitorService;
        }

        public async Task<IEnumerable<WebsiteGetDto>> GetAllWebsitesAsync(int UserID)
        {
            var websites = await _context.Websites
                                        .AsNoTracking()
                                        .Where(w => w.UserID == UserID)
                                        .ToListAsync();
            return _mapper.Map<IEnumerable<WebsiteGetDto>>(websites);
        }

        public async Task<WebsiteGetDto> GetWebsiteByIdAsync(int UserID, int id)
        {
            var website = await _context.Websites
                                        .Where(w => w.WebsiteID == id && w.UserID == UserID)
                                        .FirstOrDefaultAsync() 
                                        ?? throw new KeyNotFoundException("Website not found or you do not have access to it.");
            return _mapper.Map<WebsiteGetDto>(website);
        }
        public async Task<WebsiteGetDto> AddWebsiteAsync(WebsitePostDto websiteDto)
        {
            await ValidateWebsiteDataAsync(websiteDto);
            var website = _mapper.Map<Website>(websiteDto);
            _context.Websites.Add(website);
            await _context.SaveChangesAsync();

            await _monitorService.SetupMonitoringAsync(website);

            return _mapper.Map<WebsiteGetDto>(website);
        }

        public async Task RemoveWebsiteAsync(int id)
        {
            var website = await _context.Websites
                            .Include(w => w.MonitorLogs)
                            .FirstOrDefaultAsync(w => w.WebsiteID == id) 
                ?? throw new KeyNotFoundException("Website not found.");
            
            if(website.MonitorLogs.Count != 0){
                throw new InvalidOperationException("Monitoring logs were found. Cannot delete.");
            }

            _context.Websites.Remove(website);
            await _context.SaveChangesAsync();
        }
        public async Task ValidateWebsiteDataAsync(WebsitePostDto websiteDto)
        {
            // Duplicate check
            bool UrlExists =  await _context.Websites.AnyAsync(w => (w.Url == websiteDto.Url) && (w.UserID == websiteDto.UserID));
            if(UrlExists)
            {
                throw new InvalidOperationException("A website with the same URL already exists.");
            }
        }
    }
}
