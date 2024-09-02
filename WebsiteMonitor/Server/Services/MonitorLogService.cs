using System.Net;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Shared.DTOs;
using Shared.Models;
using Server.Database;

namespace Server.Services
{
    public interface IMonitorLogService
    {
        Task<IEnumerable<MonitorLogGetDto>> GetLogsByWebsiteIDAsync(int UserID,int WebsiteID);
        Task<MonitorLogGetDto> AddWebsiteLogAsync(int websiteID, MonitorLogPostDto MonitorLogDto);
        Task ValidateMonitorLogDataAsync(MonitorLogPostDto monitorLogDto);
    }

    public class MonitorLogService : IMonitorLogService
    {
        private readonly WebsiteMonitorContext _context;
        private readonly IMapper _mapper;

        public MonitorLogService(WebsiteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MonitorLogGetDto> AddWebsiteLogAsync(int websiteID, MonitorLogPostDto monitorLogDto)
        {
            await ValidateMonitorLogDataAsync(monitorLogDto);
            var website = await _context.Websites.FindAsync(websiteID) 
                                ?? throw new KeyNotFoundException($"Website not found.");

            var monitorLog = _mapper.Map<MonitorLog>(monitorLogDto);
            monitorLog.WebsiteID = websiteID;  // Set the WebsiteID for the new log

            _context.MonitorLogs.Add(monitorLog);
            await _context.SaveChangesAsync();

            return _mapper.Map<MonitorLogGetDto>(monitorLog);
        }

        public async Task<IEnumerable<MonitorLogGetDto>> GetLogsByWebsiteIDAsync(int UserID,int WebsiteID)
        {
            var website = await _context.Websites
                                            .Include(w => w.MonitorLogs)
                                            .FirstOrDefaultAsync(w => w.WebsiteID== WebsiteID && w.UserID == UserID) 
                                            ?? throw new KeyNotFoundException("Website not found.");

            return _mapper.Map<IEnumerable<MonitorLogGetDto>>(website.MonitorLogs.ToList());
        }
        public async Task ValidateMonitorLogDataAsync(MonitorLogPostDto monitorLogDto)
        {
            // Check if the ResponseStatus is a valid HTTP status code
            if (!Enum.IsDefined(typeof(HttpStatusCode), monitorLogDto.ResponseStatus))
            {
                throw new InvalidOperationException("ResponseStatus is not a valid HTTP status code.");
            }

            await Task.CompletedTask; // Placeholder to satisfy async method signature
        }
    }
}
