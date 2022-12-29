using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using BusinessLogic.Filtering.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogic.Filtering.AppUser;

public sealed class AppUserFilter : CustomFilterBase
{
    [FromQuery(Name = "id.eq")]
    public string Id { get; set; }

    [FromQuery(Name = "userName.stw")]
    [StringFilterOptions(StringFilterOption.StartsWith)]
    public string UserName { get; set; }

    [FromQuery(Name = "email.stw")]
    [StringFilterOptions(StringFilterOption.StartsWith)]
    public string Email { get; set; }

    [FromQuery(Name = "emailConfirmed.eq")]
    public bool? EmailConfirmed { get; set; }
}
