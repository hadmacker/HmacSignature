using System;
using FluentAssertions;
using HmacSignature;
using Xunit;

namespace HmacSignatureTests
{
    public class InstanceSignatureBuilderSignatureCalculationTests
    {
        private const string knownKey = "In software systems, it is often the early bird that makes the worm.";

        [Fact]
        public void PropertiesImplicitlyIncludedInCalculation()
        {
            //Online HMAC tester: https://www.devglan.com/online-tools/hmac-sha256-online
            var expected = new SignatureCalculation(
                "8feda6b02259091f6d58b53f5bf5645586fe38694c43d50c4e06f51265ed84a9",
                "Field1Oneman'sconstantisanotherman'svariable.Field22147483647Field39223372036854775807Field43.4028235E+38Field51.7976931348623157E+308Field69999-12-31T23:59:59.9999999Field7true"
                );

            var input = new TestModelInclusive();
            var calculator = new HMACSHA256SignatureCalculator();
            var sut = new InstanceSignatureBuilder(calculator);
            var actual = sut.Compute(input, knownKey);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PropertiesPartialExclusionsWithCalculation()
        {
            //Online HMAC tester: https://www.devglan.com/online-tools/hmac-sha256-online
            var expected = new SignatureCalculation(
                "9bac02d1fb108ec1581000627cf4b4edf99976761012a56a44d9bbb4829edba4",
                "Field3Ifalistenernodshisheadwhenyou'reexplainingyourprogram,wakehimup.Field42147483647Field5trueField6true"
            );
            var input = new TestModelPartialExclusions();
            var calculator = new HMACSHA256SignatureCalculator();
            var sut = new InstanceSignatureBuilder(calculator);
            var actual = sut.Compute(input, knownKey);
            actual.Should().BeEquivalentTo(expected);
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

        private class TestModelPartialExclusions
        {
            public TestModelPartialExclusions()
            {
                // Populating with arbitrary known constants to return a known output.
                Field1 = "One man's constant is another man's variable.";
                Field2 = int.MaxValue;
                Field3 = "If a listener nods his head when you're explaining your program, wake him up.";
                Field4 = int.MaxValue;
                Field5 = true;
                Field6 = true;
                Field7 = new object();
                Signature = "If a program manipulates a large amount of data, it does so in a small number of ways.";
            }

            [Signature(Excluded = true)] public string Field1 { get; set; }
            [Signature(Excluded = true)] public int Field2 { get; set; }
            public string Field3 { get; set; }
            public int Field4 { get; set; }
            public bool Field5 { get; set; }
            public bool Field6 { get; set; }
            // Implicitly excluded due to unrecognized type
            public object Field7 { get; set; }
            [Signature(Excluded = true)]
            public string Signature { get; set; }
        }
    }
}