
namespace ImanSoftware.Branding;

public interface IBrandingService
{
    Task<BadgeData?> GetBadgeDataAsync(string projectId, CancellationToken cancellationToken = default);
}