using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogic.Models.AppUser;

public sealed class AppUserFilter : PaginationFilterBase
{
    public string Id { get; set; }

    [FromQuery(Name = $"{nameof(UserName)}.stw")]
    [StringFilterOptions(StringFilterOption.StartsWith)]
    public string UserName { get; set; }

    [FromQuery(Name = $"{nameof(Email)}.stw")]
    [StringFilterOptions(StringFilterOption.StartsWith)]
    public string Email { get; set; }

    public bool? EmailConfirmed { get; set; }
}
