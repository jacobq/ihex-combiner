using System.Text;
using IntelHexCombiner;

namespace IntelHexCombinerTests
{
    [TestFixture]
    public class Crc32
    {
        [Test]
        public void Crc32_123456789()
        {
            // "standard" test vector of the ASCII string "123456789" (9 bytes, no null)
            // --> 0xCBF43926
            // https://crccalc.com/?crc=123456789&method=crc32&datatype=ascii&outtype=0
            String inputChars = "123456789";
            ASCIIEncoding ascii = new ASCIIEncoding();
            int byteCount = ascii.GetByteCount(inputChars.ToCharArray(), 0, inputChars.Length);
            byte[] inputBytes = new Byte[byteCount];
            int numInputBytes = ascii.GetBytes(inputChars, 0, inputChars.Length, inputBytes, 0);

            uint expected = 0xCBF43926;
            uint actual = Crc.Crc32BE(inputBytes, numInputBytes);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
