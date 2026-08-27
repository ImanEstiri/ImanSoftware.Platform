namespace ImanSoftware.Branding;

public class BrandingOptions
{
    public string PrimaryApiUrl = "https://imansoftware.ir/api/badge";
    public string BackupApiUrl = ""; 
    public string DefaultBadgeText { get; set; } = "Designed & Developed by ImanSoftware";
    public TimeSpan CacheDurationHours { get; set; } = TimeSpan.FromHours(12);
    public int RequestTimeoutSeconds { get; set; } = 3;
}