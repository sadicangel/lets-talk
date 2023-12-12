using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace LetsTalk.Services;

public static class Uuid
{
    public static string Create()
    {
        return CreateUuidV7(DateTimeOffset.UtcNow).ToString();
    }

    public static DateTimeOffset GetTimestamp(string uuid7)
    {
        Span<byte> bytes = stackalloc byte[16];
        new Guid(uuid7).TryWriteBytes(bytes, bigEndian: true, out var _);
        return DecodeUuidV7(bytes).timestamp;
    }

    [SkipLocalsInit]
    private static Guid CreateUuidV7(DateTimeOffset timestamp)
    {
        var unixMilliseconds = timestamp.ToUnixTimeMilliseconds();
        if (unixMilliseconds < 0)
            throw new ArgumentOutOfRangeException(nameof(timestamp), timestamp, "The timestamp must be after 1 January 1970.");

        // "UUIDv7 values are created by allocating a Unix timestamp in milliseconds in the most significant 48 bits ..."
        var timeHigh = (uint)(unixMilliseconds >> 16);
        var timeLow = (ushort)unixMilliseconds;

        // "... and filling the remaining 74 bits, excluding the required version and variant bits, with random bits"
        Span<byte> bytes = stackalloc byte[10];
        RandomNumberGenerator.Fill(bytes);

        var randA = (ushort)(0x7000u | (bytes[0] & 0xF) << 8 | bytes[1]);

        return new Guid(timeHigh, timeLow, randA, (byte)(bytes[2] & 0x3F | 0x80), bytes[3], bytes[4], bytes[5], bytes[6], bytes[7], bytes[8], bytes[9]);
    }

    private static (DateTimeOffset timestamp, short sequence) DecodeUuidV7(Span<byte> uuid7)
    {
        Span<byte> timestampBytes = stackalloc byte[8];
        uuid7[0..6].CopyTo(timestampBytes[2..8]);
        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(BinaryPrimitives.ReadInt64BigEndian(timestampBytes));

        var sequenceBytes = uuid7[6..8];
        //remove version information
        sequenceBytes[0] &= 0b0000_1111;
        var sequence = BinaryPrimitives.ReadInt16BigEndian(sequenceBytes);

        return (timestamp, sequence);
    }
}