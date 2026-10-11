using SharpMath.Equations;

namespace SharpMath.Tests.Equations;

public class LinearEquationSystemTests
{
    [Fact]
    public void Solve_ThreeVariables()
    {
        var system = new LinearEquationSystem(
            new LinearEquation([1.0, -1.0, 2.0], 6),
            new LinearEquation([2.0, 3.0, 2.0], 11),
            new LinearEquation([3.0, 2.0, 1.0], 8));
        var results = system.Solve();
        Assert.Equal(1, results[0], 12);
        Assert.Equal(1, results[1], 12);
        Assert.Equal(3, results[2], 12);
    }

    [Fact]
    public void Solve_SingleEquationAndParamsConstructor()
    {
        Assert.Equal(0.5, new LinearEquationSystem(new LinearEquation([2.0], 1)).Solve()[0]);
        var equation = new LinearEquation([1.0, 1.0], 6);
        Assert.Equal([1.0, 1.0], equation.Coefficients);
        Assert.Equal(6, equation.Result);
    }

    [Fact]
    public void Solve_RequiresAPivotSearchDownTheColumn()
    {
        // Regression: y + z = 5, y = 2, x = 1 used to be reported as not solvable.
        var system = new LinearEquationSystem(new List<LinearEquation>
        {
            new([0.0, 1.0, 1.0], 5),
            new([0.0, 1.0, 0.0], 2),
            new([1.0, 0.0, 0.0], 1)
        });
        Assert.Equal([1.0, 2.0, 3.0], system.Solve());
    }

    [Fact]
    public void Solve_WorksAtSmallScales()
    {
        // Regression: 1e-10·x = 1e-10 used to be rejected by an absolute epsilon.
        Assert.Equal(1, new LinearEquationSystem(new LinearEquation([1e-10], 1e-10)).Solve()[0], 12);
    }

    [Fact]
    public void Solve_ThrowsWithoutUniqueSolution()
    {
        Assert.Throws<EquationNotSolvableException>(() =>
            new LinearEquationSystem(new LinearEquation([0.0], 0)).Solve());
        Assert.Throws<EquationNotSolvableException>(() => new LinearEquationSystem(
            new LinearEquation([1.0, 2.0], 3), new LinearEquation([2.0, 4.0], 6)).Solve());
    }

    [Fact]
    public void Constructor_ValidatesTheShape()
    {
        Assert.Equal("equations", Assert.Throws<ArgumentException>(() => new LinearEquationSystem()).ParamName);

        // Regression: too many coefficients used to throw IndexOutOfRangeException; too few were padded with zeros.
        Assert.Throws<ArgumentException>(() => new LinearEquationSystem(new LinearEquation([1.0, 2.0], 3)));
        Assert.Throws<ArgumentException>(() => new LinearEquationSystem(
            new LinearEquation([1.0], 3), new LinearEquation([1.0, 2.0], 3)));
        Assert.Throws<ArgumentException>(() => new LinearEquationSystem(new LinearEquation[] { null! }));
        Assert.Throws<ArgumentNullException>(() => new LinearEquationSystem((IEnumerable<LinearEquation>) null!));
        Assert.Throws<ArgumentNullException>(() => new LinearEquation(null!, 1));
    }

    [Fact]
    public void Exception_HasStandardConstructors()
    {
        Assert.NotEmpty(new EquationNotSolvableException().Message);
        Assert.Equal("x", new EquationNotSolvableException("x").Message);
        var inner = new InvalidOperationException();
        Assert.Same(inner, new EquationNotSolvableException("x", inner).InnerException);
    }
}
