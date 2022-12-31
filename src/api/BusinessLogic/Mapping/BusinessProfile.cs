using AutoMapper;
using BusinessLogic.Models.AppUser;
using DataAccess.Entities;

namespace BusinessLogic.Mapping;

public class BusinessProfile : Profile
{
	public BusinessProfile()
	{
		CreateMap<RegisterModel, AppUser>();
		CreateMap<AppUser, UserViewModel>();

		CreateMap<PatchUserModel, AppUser>();
		CreateMap<CreateUserModel, AppUser>();
	}
}
