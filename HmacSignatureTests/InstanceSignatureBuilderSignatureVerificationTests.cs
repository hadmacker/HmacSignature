using System;
using FluentAssertions;
using HmacSignature;
using Xunit;

namespace HmacSignatureTests
{
    public class InstanceSignatureBuilderSignatureVerificationTests
    {
        private const string knownKey = "In software systems, it is often the early bird that makes the worm.";

        [Fact]
        public void PropertiesImplicitlyIncludedInCalculation()
        {
            //Online HMAC tester: https://www.devglan.com/online-tools/hmac-sha256-online
            const string signature = "8feda6b02259091f6d58b53f5bf5645586fe38694c43d50c4e06f51265ed84a9";
            var input = new TestModelInclusive();
            var calculator = new HMACSHA256SignatureCalculator();
            var sut = new InstanceSignatureBuilder(calculator);

            sut.VerifyHex(input,signature, knownKey).Should().BeTrue();
        }

        [Theory]
        [InlineData("foobarfoobarfoobarfoobarfoobarfoobarfoobarfoobarfoobarfoobarfoob")]
        [InlineData("extralongextralongextralongextralongextralongextralongextralongextralong1")]
        [InlineData("Foo")]
        [InlineData("foobar")]
        [InlineData("")]
        [InlineData(null)]
        public void BadSignatureEmptyLength(string badSignature)
        {
            //Online HMAC tester: https://www.devglan.com/online-tools/hmac-sha256-online
            var input = new TestModelInclusive();
            var calculator = new HMACSHA256SignatureCalculator();
            var sut = new InstanceSignatureBuilder(calculator);

            sut.VerifyHex(input, badSignature, knownKey).Should().BeFalse();
        }

        private class TestModelInclusive
        {
            public string Field1 { get; set; }
            public int Field2 { get; set; }
            public long Field3 { get; set; }
            public float Field4 { get; set; }
            public double Field5 { get; set; }
            public DateTime Field6 { get; set; }
            public bool Field7 { get; set; }
            // Implicitly excluded due to unrecognized type
            public object Field8 { get; set; }
            [Signature(Excluded = true)]
            public string Signature { get; set; }

            public TestModelInclusive()
            {
                // Populating with arbitrary known constants to return a known output.
                Field1 = "One man's constant is another man's variable.";
                Field2 = int.MaxValue;
                Field3 = long.MaxValue;
                Field4 = float.MaxValue;
                Field5 = double.MaxValue;
                Field6 = DateTime.MaxValue;
                Field7 = true;
                Field8 = new object(); // A property of type that will not be implicitly included in signature
                Signature = "If a program manipulates a large amount of data, it does so in a small number of ways.";
            }
        }
    }
}