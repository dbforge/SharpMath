namespace SharpMath.Tests;

public class ToleranceTests
{
    [Theory]
    [InlineData(1.0, 1.0)]
    [InlineData(0.0, 1e-13)]
    [InlineData(1e10 * (0.1 + 0.2), 3e9)]
    [InlineData(double.PositiveInfinity, double.PositiveInfinity)]
    public void AreEqual_AcceptsValuesWithinTolerance(double a, double b) => Assert.True(Tolerance.AreEqual(a, b));

    [Theory]
    [InlineData(1e-10, 1e-12)]
    [InlineData(1.0, 1.0001)]
    [InlineData(double.NaN, double.NaN)]
    [InlineData(double.PositiveInfinity, double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity, 1.0)]
    public void AreEqual_RejectsValuesOutsideTolerance(double a, double b) => Assert.False(Tolerance.AreEqual(a, b));

    [Fact]
    public void AreEqual_UsesCustomTolerances()
    {
        Assert.True(Tolerance.AreEqual(1, 1.05, 0.1, 0));
        Assert.True(Tolerance.AreEqual(100, 101, 0, 0.01));
        Assert.False(Tolerance.AreEqual(100, 102, 0, 0.01));
    }

    [Fact]
    public void AreEqual_RejectsNegativeTolerances()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Tolerance.AreEqual(1, 1, -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Tolerance.AreEqual(1, 1, 0, double.NaN));
    }

    [Fact]
    public void IsZero_UsesAbsoluteTolerance()
    {
        Assert.True(Tolerance.IsZero(1e-13));
        Assert.False(Tolerance.IsZero(1e-11));
        Assert.True(Tolerance.IsZero(0.05, 0.1));
        Assert.Throws<ArgumentOutOfRangeException>(() => Tolerance.IsZero(0, -0.1));
    }

    [Fact]
    public void IsNegligible_ScalesWithReference()
    {
        Assert.True(Tolerance.IsNegligible(1e-4, 1e6));
        Assert.False(Tolerance.IsNegligible(1e-4, 1));
        Assert.True(Tolerance.IsNegligible(1e-13, 0));
    }
}
