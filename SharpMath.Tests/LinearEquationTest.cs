using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpMath.Equations;

namespace SharpMath.Tests
{
    [TestClass]
    public class LinearEquationTest
    {
        [TestMethod]
        public void CanSolveSimpleLinearEquationSystem()
        {
            // 2x = 1
            var equation = new LinearEquation
            {
                Coefficients = new[] {2.0},
                Result = 1
            };
            var linearEquationSystem = new LinearEquationSystem(equation);
            Assert.AreEqual(0.5, linearEquationSystem.Solve()[0]);
        }

        [TestMethod]
        public void CanSolveLinearEquationSystem()
        {
            // x - y + 2z = 6
            var firstEquation = new LinearEquation(new[] { 1.0, -1.0, 2.0 }, 6);
            // 2x + 3y + 2z = 11
            var secondEquation = new LinearEquation(new[] { 2.0, 3.0, 2.0 }, 11);
            // 3x + 2y + z = 8
            var thirdEquation = new LinearEquation(new[] { 3.0, 2.0, 1.0 }, 8);

            var linearEquationSystem = new LinearEquationSystem(firstEquation, secondEquation, thirdEquation);
            double[] results = linearEquationSystem.Solve();
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(1, results[1]);
            Assert.AreEqual(3, results[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(EquationNotSolvableException))]
        public void CanNotSolveLinearEquationSystemClearly()
        {
            // 0x = 0 - infinite solutions
            var equation = new LinearEquation(new[] { 0.0 }, 0);
            var results = new LinearEquationSystem(equation).Solve(); // This will throw an exception
            Assert.AreEqual(0, results[0]);
        }
    }
}
