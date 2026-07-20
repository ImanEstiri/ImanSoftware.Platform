using ImanSoftware.Framework.Abstractions.CurrentUserAccessor;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Security.Claims;

namespace ImanSoftware.Framework.Infrastructure.CurrentUserAccessor;

/// <summary>
/// HTTP-specific implementation of ICurrentUserAccessor.
/// Uses .NET's TypeDescriptor to automatically convert the 'sub' claim into TUserId.
/// Supports int, long, Guid, string, and other primitive types out-of-the-box.
/// </summary>
public class HttpContextUserAccessor<TUserId> : ICurrentUserAccessor<TUserId>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Gets the current user's ID by reading the 'sub' claim and automatically converting it to TUserId.
    /// Returns default(TUserId) if conversion fails or user is not authenticated.
    /// </summary>
    public TUserId? GetCurrentUserId()
    {
        var user = GetCurrentUser();
        if (user is null)
            return default;

        var userIdClaim = user.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return default;

        try
        {
            var converter = TypeDescriptor.GetConverter(typeof(TUserId));
            if (converter.CanConvertFrom(typeof(string)))
            {
                return (TUserId?)converter.ConvertFromInvariantString(userIdClaim);
            }
            return default;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Gets the ClaimsPrincipal object from the current HttpContext.
    /// </summary>
    public ClaimsPrincipal? GetCurrentUser()
    {
        return _httpContextAccessor.HttpContext?.User;
    }
}