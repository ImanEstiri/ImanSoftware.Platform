using ImanSoftware.GuidGenerator;

namespace ImanSoftware.Framework.Infrastructure.GuidGenerator;

/// <summary>
/// Sequential Guid generator that creates GUIDs in chronological order 
/// to prevent database index fragmentation and improve performance.
/// </summary>
public class SequentialGuidGenerator : IGuidGenerator
{
    public Guid NewGuid()
    {
        byte[] guidArray = Guid.NewGuid().ToByteArray();
        DateTime now = DateTime.Now;

        byte[] dateTimeBytes = BitConverter.GetBytes(now.Ticks / 10000L);

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(dateTimeBytes);
        }

        Array.Copy(dateTimeBytes, dateTimeBytes.Length - 6, guidArray, guidArray.Length - 6, 6);

        return new Guid(guidArray);
    }
}
