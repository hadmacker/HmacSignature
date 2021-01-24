using System;
using HmacSignature;
using Xunit;
using Xunit.Abstractions;

namespace HmacSignatureTests
{
    public class QuickStart_BuildSignature
    {
        private readonly ITestOutputHelper _output;

        public QuickStart_BuildSignature(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuildSignatureAndVerifyExample_Hex()
        {
            // Any input. Even an anonymous type will do.
            var input = new
            {
                Id = 1,
                Name = "Hello World",
                CreatedDate = DateTime.MaxValue
            };

            // Secret key shared between the sender and receiver.
            var sharedSecretKey = "This is the key shared between the signature creator and signature verifier";

            // Sender computes signature.
            var signatureBuilder = new InstanceSignatureBuilder(new HMACSHA256SignatureCalculator());
            var signatureResult = signatureBuilder.Compute(input, sharedSecretKey);

            _output.WriteLine($"ASCII Payload used to calculate signature: {signatureResult.PayloadAsASCIIString()}");
            _output.WriteLine($"Signature in Hex: {signatureResult.SignatureAsHexString()}");

            // receiver verifies signature, positive verification when sender and receiver key matches.
            var verificationStatus = signatureBuilder.VerifyHex(input, signatureResult.SignatureAsHexString(), sharedSecretKey)
                ? "verified"
                : "incorrect";

            _output.WriteLine($"Signature Validity: {verificationStatus}");

            // receiver verifies signature, negative verification.
            // This test should fail in the case where either the signature or the key is different from original provided values.
            // In the example below, a different key is used when comparing signatures.
            var verificationStatusForcedFailure = signatureBuilder.VerifyHex(input, signatureResult.SignatureAsHexString(), "Any different key will force an incorrect verification.")
                ? "verified"
                : "incorrect";

            _output.WriteLine($"Signature Validity: {verificationStatusForcedFailure}");
        }

        [Fact]
        public void BuildSignatureAndVerifyExample_Base64()
        {
            // Any input. Even an anonymous type will do.
            var input = new
            {
                Id = 1,
                Name = "Hello World",
                CreatedDate = DateTime.MaxValue
            };

            // Secret key shared between the sender and receiver.
            var sharedSecretKey = "This is the key shared between the signature creator and signature verifier";

            // Sender computes signature.
            var signatureBuilder = new InstanceSignatureBuilder(new HMACSHA256SignatureCalculator());
            var signatureResult = signatureBuilder.Compute(input, sharedSecretKey);

            _output.WriteLine($"ASCII Payload used to calculate signature: {signatureResult.PayloadAsASCIIString()}");
            _output.WriteLine($"Signature in Base64: {signatureResult.SignatureAsBase64String()}");

            // receiver verifies signature, positive verification when sender and receiver key matches.
            var verificationStatus = signatureBuilder.VerifyBase64(input, signatureResult.SignatureAsBase64String(), sharedSecretKey)
                ? "verified"
                : "incorrect";

            _output.WriteLine($"Signature Validity: {verificationStatus}");

            // receiver verifies signature, negative verification.
            // This test should fail in the case where either the signature or the key is different from original provided values.
            // In the example below, a different key is used when comparing signatures.
            var verificationStatusForcedFailure = signatureBuilder.VerifyBase64(input, signatureResult.SignatureAsBase64String(), "Any different key will force an incorrect verification.")
                ? "verified"
                : "incorrect";

            _output.WriteLine($"Signature Validity: {verificationStatusForcedFailure} (Forced failure using mismatched secret keys)");
        }
    }
}