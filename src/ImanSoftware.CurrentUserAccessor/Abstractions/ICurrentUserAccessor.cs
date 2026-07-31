using System.Security.Claims;

namespace ImanSoftware.CurrentUserAccessor;

/// <summary>
/// Contract for accessing the currently authenticated user's information.
/// Generic type TUserId allows flexibility for different ID types (e.g., int, Guid, string).
/// </summary>
public interface ICurrentUserAccessor<out TUserId>
{
    /// <summary>
    /// Retrieves the unique identifier of the currently logged-in user.
    /// </summary>
    TUserId? GetCurrentUserId();

    /// <summary>
    /// Retrieves the complete ClaimsPrincipal object representing the current user.
    /// </summary>
    ClaimsPrincipal? GetCurrentUser();
}