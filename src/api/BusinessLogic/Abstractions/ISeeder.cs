using FluentResults;

namespace BusinessLogic.Abstractions;

public interface ISeeder
{
    Task<Result> SeedAsync();
}
