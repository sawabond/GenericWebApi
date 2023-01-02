using DataAccess.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities;

public class AppUser : IdentityUser, IEntity<string>
{
}
