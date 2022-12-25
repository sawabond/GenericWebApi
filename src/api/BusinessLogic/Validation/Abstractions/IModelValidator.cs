using FluentResults;

namespace BusinessLogic.Validation.Abstractions;

public interface IModelValidator
{
    Result Validate<T>(T model) where T : class;
}
