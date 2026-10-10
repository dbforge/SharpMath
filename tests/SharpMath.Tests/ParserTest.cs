using SharpMath.Expressions;

namespace SharpMath.Tests
{
    public class ParserTest
    {
        [Fact]
        public void CanParseTerm()
        {
            var term = "(-2+3)*(cos(0)*(2/3))";
            Assert.Equal(2d / 3d, new Parser(term).Evaluate());

            var secondTerm = "3*4^2";
            Assert.Equal(48, new Parser(secondTerm).Evaluate());
        }
    }
}