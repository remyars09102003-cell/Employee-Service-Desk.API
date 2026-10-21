using ServiceDesk.Application.Abstractions;

namespace ServiceDesk.Application.Common;

public static class CurrentUserExtensions
{
    /// <summary>Agent, manager or administrator.</summary>
    public static bool IsStaff(this ICurrentUserService user)
        => user.IsInRole(Roles.Agent) || user.IsInRole(Roles.Manager) || user.IsInRole(Roles.Administrator);

    public static int RequireEmployeeId(this ICurrentUserService user)
        => user.EmployeeId ?? throw new UnauthorizedAccessException("You must be signed in as an employee.");
}
