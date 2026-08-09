namespace ImanSoftware.RateLimiter.Core.Enums;

/// <summary>
/// Defines the partition key for rate limiting.
/// </summary>
public enum RateLimitPartitionBy
{
    /// <summary>
    /// Partition by client IP address.
    /// </summary>
    IpAddress,

    /// <summary>
    /// Partition by authenticated user ID.
    /// </summary>
    UserId,

    /// <summary>
    /// Partition by API key.
    /// </summary>
    ApiKey,

    /// <summary>
    /// Partition by endpoint path.
    /// </summary>
    Endpoint,

    /// <summary>
    /// Custom partition key (user-defined).
    /// </summary>
    Custom
}