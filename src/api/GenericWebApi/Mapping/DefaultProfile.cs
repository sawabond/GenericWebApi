using AutoMapper;
using BusinessLogic.Models.AppUser;
using GenericWebApi.Requests.Auth;

namespace GenericWebApi.Mapping;

public class DefaultProfile : Profile
{
	public DefaultProfile()
	{
		CreateMap<RegisterRequest, RegisterModel>();
	}
}
