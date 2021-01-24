using System.Security.Cryptography;
using System.Text;

namespace HmacSignature
{
    public class HMACSHA256SignatureCalculator : ISignatureCalculator
    {
        public SignatureCalculation Calculate(string payload, string key)
        {
            var payloadBytes = Encoding.ASCII.GetBytes(payload);
            var keyBytes = Encoding.ASCII.GetBytes(key);
            var result = Calculate(payloadBytes, keyBytes);
            return new SignatureCalculation(result, payloadBytes);
        }

        private byte[] Calculate(byte[] payload, byte[] key)
        {
            using var hmacSha256 = new HMACSHA256(key);
            return hmacSha256.ComputeHash(payload);
        }
    }
}
