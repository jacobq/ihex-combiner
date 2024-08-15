using System.Text; // for StringBuilder
using System; // for String
//using System.Diagnostics;
//using System.IO;
using HexIO;
using System.Diagnostics;

namespace IntelHexCombiner;

public class HexFileReader
{
    // Adapted from
    // https://stackoverflow.com/questions/10940883/converting-byte-array-to-string-and-printing-out-to-console
    public static void WriteByteArray(byte[] bytes, uint size, uint baseAddress, TextWriter writer)
    {
        const uint width = 16;
        var sb = new StringBuilder("");
        for (int i = 0; i < size; i++)
        {
            if (i % width == 0)
            {
                sb.Append("0x");
                sb.Append(String.Format("{0,8:X8}", baseAddress + i));
                sb.Append(" [");
                sb.Append(String.Format("{0,4:X4}", i));
                sb.Append("] ");
            }
            sb.Append(String.Format("{0,2:X2}", bytes[i]) + " ");
            if (i % width == width - 1)
                sb.Append("\n");
        }

        writer.WriteLine(sb.ToString());
    }

    public static void PrintByteArray(byte[] bytes, uint size, uint baseAddress)
    {
        WriteByteArray(bytes, size, baseAddress, Console.Out);
    }


    // Note from the spec (http://www.interlog.com/~speff/usefulinfo/Hexfrmt.pdf):
    //     The linear address at which a particular byte is loaded is calculated as:
    //         (LBA + DRLO + DRI) MOD 4G
    //     where:
    //         DRLO is the LOAD OFFSET field of a Data Record.
    //         DRI is the data byte index within the Data Record.
    //
    // If the caller does not specify `startAddress` we use the first extended linear address record found in the hex file.
    public static (byte[], uint, Int64) GetBytesFromHexFileStream(Stream stream, Int64 startAddress = -1, int maxSize = 256 * 1024)
    {
        byte[] bytes = new byte[maxSize];
        IIntelHexStreamReader hexStreamReader = new IntelHexStreamReader(stream);
        uint i = 0;
        bool shouldSetStartAddressAtNextRecord = startAddress < 0;
        do
        {
            IntelHexRecord intelHexRecord = hexStreamReader.ReadHexRecord();

            switch (intelHexRecord.RecordType)
            {
                case IntelHexRecordType.Data:
                    if (shouldSetStartAddressAtNextRecord)
                    {
                        startAddress = (hexStreamReader.State.UpperLinearBaseAddress << 16) + intelHexRecord.Offset;
                        Debug.WriteLine($"Setting startAddress to {string.Format("0x{0,8:X8}", startAddress)}"); // DEBUG
                        shouldSetStartAddressAtNextRecord = false;
                    }
                    // Fill "gaps" in memory (not specified by hex file) with 0xFF so the byte array is contiguous
                    uint offset = ((uint)hexStreamReader.State.UpperLinearBaseAddress << 16) + intelHexRecord.Offset - (uint)startAddress;
                    if (i != offset)
                        Debug.WriteLine($"offset = {offset}, i = {i}; {hexStreamReader.State}"); // DEBUG
                    while (i < offset)
                        bytes[i++] = (byte)0xFF;

                    for (int j = 0; j < intelHexRecord.RecordLength; j++)
                        bytes[i++] = intelHexRecord.Data[j];
                    break;
                case IntelHexRecordType.EndOfFile:
                    //Debug.WriteLine("EOF " + hexStreamReader.State); // DEBUG
                    break;
                case IntelHexRecordType.ExtendedSegmentAddress:
                    //Debug.WriteLine("ExtendedSegmentAddress " + hexStreamReader.State); // DEBUG
                    break;
                case IntelHexRecordType.StartSegmentAddress:
                    //Debug.WriteLine("StartSegmentAddress " + hexStreamReader.State); // DEBUG
                    break;
                case IntelHexRecordType.ExtendedLinearAddress:
                    //Debug.WriteLine("ExtendedLinearAddress " + hexStreamReader.State); // DEBUG
                    break;
                case IntelHexRecordType.StartLinearAddress:
                    //Debug.WriteLine("StartLinearAddress " + hexStreamReader.State); // DEBUG
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        } while (!hexStreamReader.State.Eof);
        return (bytes, i, startAddress);
    }
}
