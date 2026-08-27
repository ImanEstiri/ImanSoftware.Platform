using System.Text.Json;
using ImanSoftware.Logging;

namespace ImanSoftware.Branding.Infrastructure;

internal class BrandingService : IBrandingService
{
    private readonly HttpClient _httpClient;
    private readonly BrandingOptions _options;
    private readonly ILoggerAdapter<BrandingService>? _logger;

    public BrandingService(HttpClient httpClient, BrandingOptions options, ILoggerAdapter<BrandingService>? logger = null)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<BadgeData?> GetBadgeDataAsync(string projectId, CancellationToken cancellationToken = default)
    {
        try
        {
            var primaryUrl = $"{_options.PrimaryApiUrl}?projectId={Uri.EscapeDataString(projectId)}";
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            var response = await _httpClient.GetAsync(primaryUrl, cts.Token);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var data = JsonSerializer.Deserialize<BadgeData>(json);
                if (data is not null)
                    return data;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning("Failed to fetch from Primary API. Error : {ex} . Falling back to Backup." , ex);
        }

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(_options.RequestTimeoutSeconds));

            var response = await _httpClient.GetAsync(_options.BackupApiUrl, cts.Token);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync(cts.Token);
                var data = JsonSerializer.Deserialize<BadgeData>(json);
                if (data is not null)
                    return data;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogWarning( "Failed to fetch from Backup API. Error : {ex}",ex);
        }

        return null;
    }
}