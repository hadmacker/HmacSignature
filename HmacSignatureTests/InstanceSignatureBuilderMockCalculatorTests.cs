using System;
using FakeItEasy;
using FluentAssertions;
using HmacSignature;
using Xunit;

namespace HmacSignatureTests
{
    public class InstanceSignatureBuilderMockCalculatorTests
    {
        private const string knownKey = "In software systems, it is often the early bird that makes the worm.";

        [Fact]
        public void PropertiesImplicitlyIncludedInCalculation()
        {
            var input = new TestModelInclusive();
            var calculator = A.Fake<ISignatureCalculator>();
            A.CallTo(() => calculator.Calculate(A<string>._, A<string>._))
                .ReturnsLazily((string name, string key) => new SignatureCalculation(string.Empty, name));
            var sut = new InstanceSignatureBuilder(calculator);
            var actual = sut.Compute(input, knownKey);
            actual.PayloadAsASCIIString().Should()
                .Be(
                    "Field1Oneman'sconstantisanotherman'svariable.Field22147483647Field39223372036854775807Field43.4028235E+38Field51.7976931348623157E+308Field69999-12-31T23:59:59.9999999Field7true");
        }

        [Fact]
        public void PropertiesPartialExclusionsWithCalculation()
        {
            var input = new TestModelPartialExclusions();
            var calculator = A.Fake<ISignatureCalculator>();
            A.CallTo(() => calculator.Calculate(A<string>._, A<string>._))
                .ReturnsLazily((string name, string key) => new SignatureCalculation(string.Empty, name));
            var sut = new InstanceSignatureBuilder(calculator);
            var actual = sut.Compute(input, knownKey);
            actual.PayloadAsASCIIString().Should()
                .Be(
                    "Field3Ifalistenernodshisheadwhenyou'reexplainingyourprogram,wakehimup.Field42147483647Field5trueField6true");
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