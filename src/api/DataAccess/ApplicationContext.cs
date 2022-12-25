using DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class ApplicationContext : IdentityDbContext<AppUser, AppRole, string>
{
	public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
	{

	}
}
