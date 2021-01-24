namespace HmacSignature
{
    public interface ISignatureCalculator
    {
        SignatureCalculation Calculate(string payload, string key);
    }
}