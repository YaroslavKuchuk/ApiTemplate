using System;
using System.Text;

namespace Services.Helpers
{
    public static class Crypto
    {
        public static string ConvertStringToHex(string input)
        {
            Byte[] stringBytes = Encoding.Unicode.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string ConvertHexToString(string hexInput)
        {
            int numberChars = hexInput.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
