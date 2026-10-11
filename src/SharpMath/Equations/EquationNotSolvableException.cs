namespace SharpMath.Equations;

/// <summary>
///     The exception that is thrown when an equation system has no unique solution.
/// </summary>
public class EquationNotSolvableException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EquationNotSolvableException" /> class.
    /// </summary>
    public EquationNotSolvableException() : base("The equation system has no unique solution.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EquationNotSolvableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EquationNotSolvableException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EquationNotSolvableException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public EquationNotSolvableException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
