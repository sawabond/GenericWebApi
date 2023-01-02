using DataAccess;
using DataAccess.Abstractions;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Repositories;

internal sealed class UserRepository : Repository<AppUser>, IUserRepository
{
    public UserRepository(ApplicationContext context) : base(context) { }

    private DbSet<AppUser> Users => Context.Users;

    public Task<IEnumerable<AppUser>> GetUsersIncludingAll()
    {
        throw new NotImplementedException();
    }
}
