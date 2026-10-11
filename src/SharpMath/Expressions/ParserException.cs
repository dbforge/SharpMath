namespace SharpMath.Expressions;

/// <summary>
///     The exception that is thrown when an expression cannot be parsed.
/// </summary>
public class ParserException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ParserException" /> class.
    /// </summary>
    public ParserException() : this("The expression is invalid.")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParserException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ParserException(string message) : base(message)
    {
        Position = -1;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParserException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that caused this exception.</param>
    public ParserException(string message, Exception innerException) : base(message, innerException)
    {
        Position = -1;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ParserException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="position">The zero-based position in the expression where the error was detected.</param>
    public ParserException(string message, int position) : base($"{message} (at position {position})")
    {
        Position = position;
    }

    /// <summary>
    ///     Gets the zero-based position in the expression where the error was detected, or −1 if unknown.
    /// </summary>
    public int Position { get; }
}
