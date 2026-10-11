using System.Globalization;
using System.Text;

namespace SharpMath.Geometry;

/// <summary>
///     Shared helpers for the vector types.
/// </summary>
internal static class VectorMath
{
    /// <summary>
    ///     Computes the angle between two unit vectors from the length of their cross product (or of the parallelogram
    ///     they span) and their dot product. Unlike <c>acos</c>, this is accurate for (nearly) parallel vectors.
    /// </summary>
    public static double Angle(double sine, double cosine) => Math.Atan2(sine, cosine);

    /// <summary>
    ///     Determines whether a sine or cosine of two unit vectors is negligible (|value| ≤ 1e-9), independent of the
    ///     lengths of the original vectors.
    /// </summary>
    public static bool IsNegligible(double unitValue) => Math.Abs(unitValue) <= Tolerance.DefaultRelative;

    public static InvalidOperationException ZeroVectorAngle() =>
        new("The angle to or from the zero vector is undefined.");

    public static string ToLaTeX(ReadOnlySpan<double> components)
    {
        var builder = new StringBuilder(@"\left( \begin{array}{c} ");
        for (var i = 0; i < components.Length; i++)
        {
            if (i > 0)
                builder.Append(@" \\ ");
            builder.Append(components[i].ToString(CultureInfo.InvariantCulture));
        }

        return builder.Append(@" \end{array} \right)").ToString();
    }
}
