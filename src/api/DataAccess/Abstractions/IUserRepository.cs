using DataAccess.Entities;

namespace DataAccess.Abstractions;

public interface IUserRepository : IRepository<AppUser>
{
    public Task<IEnumerable<AppUser>> GetUsersIncludingAll();
}
