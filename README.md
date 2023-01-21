# GenericWebApi

This is a generic web project template. Users can clone it or create new repositories and extend it with domain logic for their purposes.

## About the project

- ASP.NET Core Web API project
- 3-layer architecture
- Entity Framework Core code first with 3 DBs allowed: In memory, SQL Server, PostgreSQL
- JWT Bearer authorization
- FluentResults, FluentValidation, AutoMapper, AutoFilterer
- XUnit, Moq, FluentAssertions, EF Core InMemory for testing
- SendGrid API for email sending

## Current features

- Role-based auth using basic 'Admin', 'Moder' and 'User' roles
- Account confirmation through email
- Google authentication
- Admin controller for user CRUD
- Unit and integration test coverage
- CI/CD process and Azure publishing action

## Features planned

- Generic UI

## Usage

Clone it to your PC and change the remote or click _Use this template_ -> _Create a new repository_

### Feature management

Some features are implemented with feature flags. To turn it on use `appsettings.json` section

```json
"FeatureManagement": {
    "EmailVerification": true,
    "GoogleAuthentication": true
  }
```

### Services

Services are added in `AddBusinessLogicServices` method with [Scrutor](https://github.com/khellang/Scrutor), so you don't need to add them explicitly. Services are added only from business layer assembly.

```C#
services.Scan(selector => selector
                .FromAssemblies(typeof(AssemblyReference).Assembly)
                .AddClasses(filter =>
                {
                    filter.NotInNamespaceOf<ModelsNamespaceReference>();
                    filter.NotInNamespaceOf<MailSettingsOptions>();
                }, publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
```

### Databases

The database type section in `appsettings.json` is for choosing a db type on start-up.

```json
"DatabaseType": "SqlServer",
```

Allowed values are:

```C#
public enum DatabaseType
{
    SqlServer = 0,
    PostgreSql = 1,
    InMemory = 2,
}
```

### Migrations

There are 2 migration assemblies to provide the ability to use different DBs - `SqlServerMigrations` and `PostgresMigrations`.</br>
To add a migration:</br>

Add Migration With CLI Command:</br>

```Powershell
dotnet ef migrations add NewMigration --project YourAssemblyName
```

Add Migration With PMC Command:</br>

```Powershell
Add-Migration NewMigration -Project YourAssemblyName
```

### Validation

Validation is implemented using `IModelValidator` interface. It finds appropriate validators using reflection and validates **business layer models**

```C#
public interface IModelValidator
{
    Result Validate<T>(T model) where T : class;
}
```

You can just add a validator to business layer assembly and it will be used by the `IModelValidator` implementation. <br>
Validation type is manual

```C#
var validationResult = _validator.Validate(model);
if (!validationResult.IsSuccess) return validationResult;
```

## CI/CD

These files can be viewed in github actions</br>
Unit and integration test running and report publishing are present on PRs to dev</br>
Deployment to Azure as Web Service is present</br>

## Contributing

Pull requests are welcome

## Project status

I work on it when I have free time and I want
