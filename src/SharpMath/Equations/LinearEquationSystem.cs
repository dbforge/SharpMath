using System.Collections.ObjectModel;
using SharpMath.Geometry;

namespace SharpMath.Equations;

/// <summary>
///     Represents an immutable system of <c>n</c> linear equations in <c>n</c> variables.
/// </summary>
public sealed class LinearEquationSystem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearEquationSystem" /> class.
    /// </summary>
    /// <param name="equations">The equations; at least one, each with exactly one coefficient per equation.</param>
    /// <exception cref="ArgumentException">The equations do not form a square system.</exception>
    public LinearEquationSystem(IEnumerable<LinearEquation> equations)
    {
        ArgumentNullException.ThrowIfNull(equations);
        Equations = Validate(equations.ToArray());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearEquationSystem" /> class.
    /// </summary>
    /// <param name="equations">The equations; at least one, each with exactly one coefficient per equation.</param>
    /// <exception cref="ArgumentException">The equations do not form a square system.</exception>
    public LinearEquationSystem(params ReadOnlySpan<LinearEquation> equations)
    {
        Equations = Validate(equations.ToArray());
    }

    /// <summary>
    ///     Gets the equations.
    /// </summary>
    public ReadOnlyCollection<LinearEquation> Equations { get; }

    /// <summary>
    ///     Solves the system using an LU decomposition with partial pivoting.
    /// </summary>
    /// <returns>The value of each variable, in the order of the coefficients.</returns>
    /// <exception cref="EquationNotSolvableException">The system has no unique solution.</exception>
    public double[] Solve()
    {
        var count = Equations.Count;
        var coefficients = new double[count * count];
        var results = new double[count];
        for (var row = 0; row < count; row++)
        {
            Equations[row].Coefficients.CopyTo(coefficients, row * count);
            results[row] = Equations[row].Result;
        }

        var matrix = MatrixMxN.WrapRowMajor(count, count, coefficients);
        if (!matrix.TrySolve(results, out var solution))
            throw new EquationNotSolvableException("The linear equation system has no unique solution.");
        return solution;
    }

    private static ReadOnlyCollection<LinearEquation> Validate(LinearEquation[] equations)
    {
        if (equations.Length == 0)
            throw new ArgumentException("There must be at least one equation.", nameof(equations));
        for (var row = 0; row < equations.Length; row++)
        {
            var equation = equations[row] ??
                           throw new ArgumentException($"Equation {row} is null.", nameof(equations));
            if (equation.Coefficients.Count != equations.Length)
                throw new ArgumentException(
                    $"Equation {row} has {equation.Coefficients.Count} coefficients, but the system has {equations.Length} equations. " +
                    "Each equation needs exactly one coefficient per variable.", nameof(equations));
        }

        return Array.AsReadOnly(equations);
    }
}
