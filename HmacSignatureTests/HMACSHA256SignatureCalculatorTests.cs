using FluentAssertions;
using HmacSignature;
using Xunit;

namespace HmacSignatureTests
{
    public class HMACSHA256SignatureCalculatorTests
    {
        [Fact]
        public void CalculatesSignatureFromKnownArbitraryStrings()
        {
            // Quote attribution: http://www.cs.yale.edu/homes/perlis-alan/quotes.html
            var actual = new HMACSHA256SignatureCalculator()
                .Calculate("One man's constant is another man's variable", "Syntactic sugar causes cancer of the semicolon");

            // Independently verified online: https://www.devglan.com/online-tools/hmac-sha256-online
            actual.SignatureAsHexString().Should().Be("17575fd12f3b1bbda8016357a7ae3d2ed554f2d323d23f887e39c970eb26350c");
        }
    }
}
