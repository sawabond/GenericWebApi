using BusinessLogic.Mapping;
using BusinessLogic.Models.AppUser;
using DataAccess.Entities;

namespace BusinessLogic.Tests.Helpers;

internal class TestProfile : BusinessProfile
{
	public TestProfile() : base()
	{
		CreateMap<AppUser, UserPatchModel>();
		CreateMap<AppUser, UserCreateModel>().ForCtorParam(ctorParamName: "Password", x => x.MapFrom(y => y.PasswordHash));
	}
}
