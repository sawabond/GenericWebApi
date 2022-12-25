# GenericWebApi

This is a generic web project template. Users can clone it or create new repositories and extend it with domain logic for their purposes.

## About the project

- ASP.NET Core Web API project
- 3-layer architecture
- Entity Framework Core code first
- JWT Bearer auth
- FluentValidation, AutoMapper

## Current features

- Authorization without roles

## Features planned

- Google auth
- Role-based auth
- Admin controller for user CRUD
- Account confirmation

## Usage

Clone it to your PC and change the remote or click _Use this template_ -> _Create a new repository_

### Services

Services are added in `AddBusinessLogicServices` method with [Scrutor](https://github.com/khellang/Scrutor), so you don't need to add them explicitly. Services are added only from business layer assembly.

```
services.Scan(selector => selector
                .FromAssemblies(typeof(AssemblyReference).Assembly)
                .AddClasses(filter => filter.NotInNamespaceOf<ModelsNamespaceReference>(), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
```

### Validation

Validation is implemented using `IModelValidator` interface. It finds appropriate validators using reflection and validates **business layer models**

```
public interface IModelValidator
{
    Result Validate<T>(T model) where T : class;
}
```

You can just add a validator to business layer assembly and it will be used by the `IModelValidator` implementation. <br>
Validation type is manual

```
var validationResult = _validator.Validate(model);
if (!validationResult.IsSuccess) return validationResult;
```

## Contributing

Pull requests are welcome

## Project status

I work on it when I have free time and I want
