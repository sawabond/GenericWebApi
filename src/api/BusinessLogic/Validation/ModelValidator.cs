using BusinessLogic.Validation.Abstractions;
using FluentResults;
using FluentValidation;
using FluentValidation.Internal;

namespace BusinessLogic.Validation;

internal sealed class ModelValidator : IModelValidator
{
    private readonly IEnumerable<IValidator> _validators;

    public ModelValidator(IEnumerable<IValidator> validators)
    {
        _validators = validators;
    }

    public Result Validate<T>(T model) where T : class
    {
        Func<IValidator, bool> matchesType = 
            validator => validator.GetType().BaseType.GenericTypeArguments.FirstOrDefault().Equals(typeof(T));

        var validators = _validators.Where(matchesType);

        var errors = new List<string>();

        foreach (var validator in validators)
        {
            var result = validator.Validate(
                new ValidationContext<T>(
                    model,
                    new PropertyChain(),
                    ValidatorOptions.Global.ValidatorSelectors.DefaultValidatorSelectorFactory()));

            if (!result.IsValid)
            {
                result.Errors.ForEach(err => errors.Add(err.ErrorMessage));
            }
        }

        return errors.Any()
            ? Result.Fail(errors)
            : Result.Ok();
    }
}
