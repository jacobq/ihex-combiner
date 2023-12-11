namespace IntelHexCombiner
{
    public class Program
    {
        private Int64 _appStartAddress = -1;
        private uint _fwImageBytes = 0;
        private byte[] _fwImage;
        
        public static void Main(string[] args)
        {
            Console.WriteLine($"{args}");
            //List<int> fibNumbers = [0, 1, 1, 2, 3, 5, 8, 13];
            //foreach (int element in fibNumbers)
            //{
            //    Console.Write($"{element} ");
            //}            
            Program instance = new Program();
            foreach (string arg in args)
            {
                instance.LoadHexFile();
            }
        }
    
        public void LoadHexFile(string hexFilePath)
        {
            Stream stream = new FileStream(hexFilePath, FileMode.Open);
            LoadHexFromFileStream(stream);
            HexFileReader.PrintByteArray(_fwImage, _fwImageBytes, (uint)_appStartAddress);
            stream.Close();
        }
        
        private void LoadHexFromFileStream(Stream stream)
        {
            (_fwImage, _fwImageBytes, _appStartAddress) = HexFileReader.GetBytesFromHexFileStream(stream);
            Console.WriteLine($"LoadHexFromFileStream got {_fwImageBytes} bytes, startAddress={String.Format("0x{0,8:X8}", _appStartAddress)}"); // DEBUG
        }
    }
}

