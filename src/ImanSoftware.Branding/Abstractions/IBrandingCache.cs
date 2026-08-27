
namespace ImanSoftware.Branding;

public interface IBrandingCache
{
    Task<BadgeData?> GetAsync(string key);
    Task SetAsync(string key, BadgeData value);
}
