using ImanSoftware.GuidGenerator;

namespace ImanSoftware.Framework.Infrastructure.GuidGenerator;

internal class RandomGuidGenerator : IGuidGenerator
{
    public Guid NewGuid()
        => Guid.NewGuid();
}
