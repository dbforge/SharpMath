namespace SharpMath.Canvas.Tests;

public class TicksTests
{
    [Theory]
    [InlineData(20, 20, 1)]
    [InlineData(80, 20, 5)]
    [InlineData(40, 20, 2)]
    [InlineData(20, 2000, 0.01)]
    [InlineData(20, 0.001, 20000)]
    [InlineData(80, 16, 5)]
    [InlineData(81, 16, 10)]
    public void NiceStep_Uses1_2_5(double minPixels, double scale, double expected) =>
        Assert.Equal(expected, Ticks.NiceStep(minPixels, scale), 9);

    [Theory]
    [InlineData(80, 16, 2, 10)]
    [InlineData(80, 20, 1, 5)]
    [InlineData(80, 40, 0.5, 2)]
    [InlineData(80, 18, 2, 10)]
    [InlineData(20, 20, 1, 1)]
    [InlineData(80, 0.001, 20000, 100000)]
    public void NiceMultiple_IsAMultipleOfTheGridStep(double minPixels, double scale, double gridStep, double expected)
    {
        var step = Ticks.NiceMultiple(minPixels, scale, gridStep);
        Assert.Equal(expected, step, 9);
        Assert.Equal(Math.Round(step / gridStep), step / gridStep, 9);
    }

    [Theory]
    [InlineData(2.5, 1, "2.5")]
    [InlineData(5, 1, "5.0")]
    [InlineData(0.123456, 0.01, "0.123")]
    [InlineData(-1234.5678, 100, "-1234.6")]
    public void FormatValue_UsesOneMoreDecimalThanTheStep(double value, double step, string expected) =>
        Assert.Equal(expected, Ticks.FormatValue(value, step));

    [Fact]
    public void GetNearestScaleRegression_LargeAndSmallSteps()
    {
        // Regression: the old algorithm normalized with (d − 1) · 10 instead of 10^(d − 1): 400 → 200, 4000 → 300.
        Assert.Equal(500, Ticks.NiceStep(400, 1), 9);
        Assert.Equal(5000, Ticks.NiceStep(4000, 1), 9);
        Assert.Equal(0.005, Ticks.NiceStep(0.004, 1), 12);
    }

    [Theory]
    [InlineData(0, 1, "0")]
    [InlineData(3, 1, "3")]
    [InlineData(-3, 0.5, "-1.5")]
    [InlineData(7, 0.1, "0.7")]
    [InlineData(5, 0.02, "0.10")]
    [InlineData(2, 5e7, "1E+8")]
    [InlineData(3, 1e-7, "3E-7")]
    public void Format_UsesStepDecimalsAndInvariantCulture(long index, double step, string expected) =>
        Assert.Equal(expected, Ticks.Format(index, step));

    [Fact]
    public void Range_IsEmptyWhenTooLargeOrImprecise()
    {
        Assert.Equal((-2L, 3L), Ticks.Range(-2.5, 3.5, 1));
        Assert.Equal((1L, 0L), Ticks.Range(0, 1e9, 1));
        Assert.Equal((1L, 0L), Ticks.Range(1e20, 1e20 + 1, 1));
        Assert.Equal((1L, 0L), Ticks.Range(double.NaN, 1, 1));
        Assert.Equal(1, Ticks.SafeSpacing(0));
        Assert.Equal(1, Ticks.SafeSpacing(double.NaN));
        Assert.Equal(25, Ticks.SafeSpacing(25));
    }
}
