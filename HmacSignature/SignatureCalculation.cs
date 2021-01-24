using System;
using System.Text;

namespace HmacSignature
{
    public class SignatureCalculation
    {

        public byte[] SignatureBytes { get; }
        public byte[] Payload { get; }

        public SignatureCalculation(byte[] signatureBytes, byte[] payload)
        {
            SignatureBytes = signatureBytes;
            Payload = payload;
        }

        public SignatureCalculation(string signature, string payload)
        {
            SignatureBytes = HexDecode(signature);
            Payload = Encoding.ASCII.GetBytes(payload);
        }

        public string SignatureAsHexString()
        {
            return BitConverter.ToString(SignatureBytes).Replace("-", string.Empty).ToLowerInvariant();
        }

        public string SignatureAsBase64String()
        {
            return Convert.ToBase64String(SignatureBytes);
        }

        public string PayloadAsASCIIString()
        {
            return Encoding.ASCII.GetString(Payload);
        }

        public static bool TryBase64Decode(string base64, out byte[] signatureBytes)
        {
            try
            {
                signatureBytes = Convert.FromBase64String(base64);
                return true;
            }
            catch
            {
                signatureBytes = new byte[0];
                return false;
            }
        }

        public static bool TryHexDecode(string hex, out byte[] signatureBytes)
        {
            try
            {
                signatureBytes = HexDecode(hex);
                return true;
            }
            catch
            {
                signatureBytes = new byte[0];
                return false;
            }
        }

        public static byte[] HexDecode(string hex)
        {
            if(hex.Length %2 != 0) 
                throw new ArgumentException($"Cannot hex decode an odd-length string. {hex.Length} characters total.");

            if(string.IsNullOrWhiteSpace(hex))
                return new byte[0];

            var bytes = new byte[hex.Length / 2];
            for (var i = 0; i < hex.Length; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}