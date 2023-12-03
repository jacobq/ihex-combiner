using System;
using System.IO.Hashing;

namespace IntelHexCombiner
{
    public class Crc
    {
        /// Wrapper for [System.IO.Hashing](https://www.nuget.org/packages/System.IO.Hashing/)'s
        /// [Crc32](https://learn.microsoft.com/en-us/dotnet/api/system.io.hashing.crc32).
        /// @return byte array in little-endian format
        private static byte[] calcCrc32(byte[] inputByteArray, int length)
        {
            Span<byte> inputByteSpan = new Span<byte>(inputByteArray);
            return Crc32.Hash(inputByteSpan.Slice(0, length));
        }

        /// Calculates CRC-32
        /// ([as used in Ethernet frame check](https://crccalc.com/?crc=123456789&method=CRC-32&datatype=hex&outtype=0))
        /// and formats it as an unsigned integer in either little-endian or big-endian format.
        /// @param[in] inputByteArray Input data to be used for CRC calculation
        /// @param[in] length Number of bytes that should be read from the input array (defaults to inputByteArray.length)
        /// @return CRC as unsigned integer in big-endian format
        public static uint Crc32BE(byte[] inputByteArray, int length = -1)
        {
            if (length < 0)
                length = inputByteArray.Length;
            byte[] crcBytes = calcCrc32(inputByteArray, length);

            return
                (uint)(crcBytes[3] << 24) |
                (uint)(crcBytes[2] << 16) |
                (uint)(crcBytes[1] << 8) |
                (uint)crcBytes[0]
            ;
        }
    }
}
