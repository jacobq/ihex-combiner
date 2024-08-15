namespace IntelHexCombiner;

class BinaryImage
{
    Int64 appStartAddress = -1;
    uint fwImageBytes = 0;
    byte[] fwImage;

    public BinaryImage(Stream stream, Int64 startAddress = -1, int maxSize = 4 * 1024 * 1024)
    {
        (fwImage, fwImageBytes, appStartAddress) = HexFileReader.GetBytesFromHexFileStream(stream, startAddress, maxSize);
    }

    public uint CalcCRC()
    {
        uint numBytesProtectedByCrc = fwImageBytes - 4;
        uint expectedCrc = Crc.Crc32BE(fwImage, (int)numBytesProtectedByCrc);
        // DEBUG
        //HexFileReader.PrintByteArray(fwImage, fwImageBytes, (uint)appStartAddress);
        //Console.WriteLine($"appStartAddress = {appStartAddress}, fwImageBytes = {fwImageBytes}, numBytesProtectedByCrc = {numBytesProtectedByCrc}");
        //Console.WriteLine($"Expected/Calculated CRC = {String.Format("0x{0,8:X8}", expectedCrc)}, (-1 {String.Format("0x{0,8:X8}", Crc.Crc32BE(fwImage, (int)numBytesProtectedByCrc - 1))}), (+1 {String.Format("0x{0,8:X8}", Crc.Crc32BE(fwImage, (int)numBytesProtectedByCrc + 1))})");
        return expectedCrc;
    }

    public void WriteCrcBE(uint crc)
    {
        fwImage[fwImageBytes - 4] = (byte)((crc >> 24) & 0xFF);
        fwImage[fwImageBytes - 3] = (byte)((crc >> 16) & 0xFF);
        fwImage[fwImageBytes - 2] = (byte)((crc >> 8) & 0xFF);
        fwImage[fwImageBytes - 1] = (byte)((crc >> 0) & 0xFF);
    }

    public void WriteCrcLE(uint crc)
    {
        fwImage[fwImageBytes - 4] = (byte)((crc >> 0) & 0xFF);
        fwImage[fwImageBytes - 3] = (byte)((crc >> 8) & 0xFF);
        fwImage[fwImageBytes - 2] = (byte)((crc >> 16) & 0xFF);
        fwImage[fwImageBytes - 1] = (byte)((crc >> 24) & 0xFF);
    }


    public void SaveHex(Stream stream)
    {
        HexFileWriter.WriteBytesToHexFileStream(fwImage, stream, appStartAddress);
    }
}

