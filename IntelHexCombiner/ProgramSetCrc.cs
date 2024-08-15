using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace IntelHexCombiner;
public class ProgramSetCrc
{
    public static void Main(string[] args)
    {
        Int64 startAddress = 0x08008000;
        int size = 80 * 1024;

        Console.WriteLine($"pem-fw-crc-writer.exe <input.hex> <output.hex> <start address in base 16> <size/bytes in base 10>");

        char[] test = "123456789".ToCharArray();
        byte[] test_bytes = Encoding.ASCII.GetBytes(test);
        Debug.WriteLine($"CRC test vector: crc(\"{test}\") = {String.Format("0x{0,8:X8}", Crc.Crc32BE(test_bytes, 9))}");
        if (args.Length >= 1)
        {
            String inputFile = args[0];
            String outputFile = args.Length >= 2 ? args[1] : inputFile.Replace(".hex", "-mod.hex");
            if (args.Length >= 3)
            {
                startAddress = Int64.Parse(args[2], NumberStyles.HexNumber);
                if (args.Length >= 4)
                {
                    size = int.Parse(args[3], NumberStyles.Number);
                }
            }

            Console.WriteLine($"Will read {inputFile} to memory, calculate its CRC (ignoring last 4 bytes), write that CRC in the last 4 bytes, and save the output as {outputFile}.");
            BinaryImage image = LoadHexFile(inputFile, startAddress, size);
            SaveHexFile(image, outputFile);
        }
        else
        {
            Console.Error.WriteLine($"ERROR: args.Length = {args.Length}");
        }
    }

    static private BinaryImage LoadHexFile(string inputHexFilePath, Int64 startAddress, int size)
    {
        Debug.WriteLine($"LoadHexFile({inputHexFilePath})");
        Stream stream = new FileStream(inputHexFilePath, FileMode.Open);
        BinaryImage image = new BinaryImage(stream, startAddress, size);
        stream.Close();
        return image;
    }

    static private void SaveHexFile(BinaryImage image, string outHexFile)
    {
        Debug.WriteLine($"SaveHexFile({outHexFile})");
        uint crc = image.CalcCRC();
        Console.WriteLine($"crc = {String.Format("0x{0,8:X8}", crc)}");
        image.WriteCrcLE(crc);
        Stream stream = new FileStream(outHexFile, FileMode.Create);
        image.SaveHex(stream);
        stream.Close();
    }
}

