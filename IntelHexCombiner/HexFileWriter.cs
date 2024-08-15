using System; // for String
using HexIO;

namespace IntelHexCombiner;

public class HexFileWriter
{
    // Note from the spec (http://www.interlog.com/~speff/usefulinfo/Hexfrmt.pdf):
    //     The linear address at which a particular byte is loaded is calculated as:
    //         (LBA + DRLO + DRI) MOD 4G
    //     where:
    //         DRLO is the LOAD OFFSET field of a Data Record.
    //         DRI is the data byte index within the Data Record.
    //
    // If the caller does not specify `startAddress` we use the first extended linear address record found in the hex file.
    public static void WriteBytesToHexFileStream(byte[] bytes, Stream stream, Int64 startAddress, Int64 size = -1)
    {
        IIntelHexStreamWriter hexStreamWriter = new IntelHexStreamWriter(stream);
        UInt16 upperAddress = (UInt16)((startAddress >> 16) & 0xFFFF);
        UInt16 lowerAddress = (UInt16)((startAddress >>  0) & 0xFFFF);
        hexStreamWriter.WriteExtendedLinearAddressRecord(upperAddress);
        //hexStreamWriter.WriteStartLinearAddressRecord(lowerAddress);
        List<byte> list = bytes.ToList();
        int i = 0; // index of bytes[] & list
        const int blockSize = 16; // 1-255
        if (size < 0)
            size = bytes.Length;
        while (i < size)
        {
            int n = bytes.Length - i;
            if (n > blockSize)
                n = blockSize;
            hexStreamWriter.WriteDataRecord(lowerAddress, list.Slice(i, n));
            i += n;
            if ((int)lowerAddress + n > 65535)
            {
                upperAddress += 1;
                lowerAddress = 0;
                hexStreamWriter.WriteExtendedLinearAddressRecord(upperAddress);
                //hexStreamWriter.WriteStartLinearAddressRecord(lowerAddress);
            }
            else
            {
                lowerAddress += (UInt16)n;
            }
        }
        hexStreamWriter.Close();
    }
}
