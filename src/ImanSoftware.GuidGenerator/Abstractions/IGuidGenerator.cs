
namespace ImanSoftware.GuidGenerator;

/// <summary>
/// Contract for generating unique identifiers. 
/// Uses SequentialGuid to prevent database index fragmentation.
/// </summary>
public interface IGuidGenerator
{
    /// <summary>
    /// Generates a new unique identifier (Guid) based on the configured strategy.
    /// </summary>
    Guid NewGuid();
}
