using AutoMapper;
using Shared.DTOs;
using Shared.Models;

namespace Server.Config;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Website, WebsiteGetDto>();
        CreateMap<WebsitePostDto, Website>();

        CreateMap<MonitorLog, MonitorLogGetDto>();
        CreateMap<MonitorLogPostDto, MonitorLog>();

        CreateMap<User, UserGetDto>();
        CreateMap<UserPostDto, User>();
    }
}
