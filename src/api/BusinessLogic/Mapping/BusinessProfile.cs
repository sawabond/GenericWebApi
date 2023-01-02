using AutoMapper;
using BusinessLogic.Models.AppUser;
using DataAccess.Entities;

namespace BusinessLogic.Mapping;

public class BusinessProfile : Profile
{
	public BusinessProfile()
	{
		CreateMap<UserRegisterModel, AppUser>();
		CreateMap<AppUser, UserAuthModel>();
        CreateMap<AppUser, UserViewModel>();

        CreateMap<UserPatchModel, AppUser>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember is not null));
        CreateMap<UserCreateModel, AppUser>();
	}
}
