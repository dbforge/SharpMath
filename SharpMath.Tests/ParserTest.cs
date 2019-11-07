// ParserTest.cs, 07.11.2019
// Copyright (C) Dominic Beger 07.11.2019

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
            var term = "(-2+3)*(cos(0)*(2/3))";
            Assert.AreEqual(2d / 3d, new Parser(term).Evaluate());

            var secondTerm = "3*4^2";
            Assert.AreEqual(48, new Parser(secondTerm).Evaluate());
        }
    }
}