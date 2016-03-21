// Author: Dominic Beger (Trade/ProgTrade) 2016

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Expressions;

namespace SharpMath.Tests
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void CanParseTerm()
        {
            string term = "(-2+3)*(cos(0)*(2/3))";
            Assert.AreEqual((2d/3d), new Parser(term).Evaluate());

            string secondTerm = "3*4^2";
            Assert.AreEqual(48, new Parser(secondTerm).Evaluate());
        }
    }
}