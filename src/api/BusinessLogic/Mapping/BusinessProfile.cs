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

		CreateMap<PatchUserModel, AppUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<CreateUserModel, AppUser>();
	}
}
