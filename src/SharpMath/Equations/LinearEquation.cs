using System.Collections.ObjectModel;

namespace SharpMath.Equations;

/// <summary>
///     Represents an immutable linear equation <c>a₁·x₁ + a₂·x₂ + … + aₙ·xₙ = result</c>.
/// </summary>
public sealed class LinearEquation
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LinearEquation" /> class.
    /// </summary>
    /// <param name="coefficients">The coefficients of the variables, in order, e.g. <c>[2.0, -1, 3]</c>.</param>
    /// <param name="result">The right-hand side.</param>
    public LinearEquation(IEnumerable<double> coefficients, double result)
    {
        ArgumentNullException.ThrowIfNull(coefficients);
        Coefficients = Array.AsReadOnly(coefficients.ToArray());
        Result = result;
    }

    /// <summary>
    ///     Gets the coefficients of the variables, in order.
    /// </summary>
    public ReadOnlyCollection<double> Coefficients { get; }

    /// <summary>
    ///     Gets the right-hand side.
    /// </summary>
    public double Result { get; }
}
