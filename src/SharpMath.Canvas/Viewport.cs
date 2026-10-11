using SharpMath.Geometry;

namespace SharpMath.Canvas;

/// <summary>
///     Maps between world coordinates (Y up) and screen coordinates (pixels, Y down), and implements panning and zooming.
/// </summary>
public sealed class Viewport
{
    /// <summary>
    ///     The smallest allowed scale in pixels per unit.
    /// </summary>
    public const double MinScale = 1e-6;

    /// <summary>
    ///     The largest allowed scale in pixels per unit.
    /// </summary>
    public const double MaxScale = 1e9;

    private bool _originSet;
    private CanvasPoint _panBeforeSizing;
    private bool _sized;

    /// <summary>
    ///     Occurs when the origin, the scale or the size changes.
    /// </summary>
    public event EventHandler? Changed;

    /// <summary>
    ///     Gets the screen position of the world origin (0, 0). Until it is set or the viewport is panned or zoomed, the
    ///     origin is centered whenever the size changes. Panning before the first <see cref="Resize" /> is applied relative
    ///     to the centered origin.
    /// </summary>
    public CanvasPoint Origin { get; private set; }

    /// <summary>
    ///     Gets the horizontal scale in pixels per unit.
    /// </summary>
    public double ScaleX { get; private set; } = 20;

    /// <summary>
    ///     Gets the vertical scale in pixels per unit.
    /// </summary>
    public double ScaleY { get; private set; } = 20;

    /// <summary>
    ///     Gets the width of the visible area in pixels.
    /// </summary>
    public double Width { get; private set; }

    /// <summary>
    ///     Gets the height of the visible area in pixels.
    /// </summary>
    public double Height { get; private set; }

    /// <summary>
    ///     Gets the visible world area.
    /// </summary>
    public BoundingBox2D VisibleArea
    {
        get
        {
            var topLeft = ScreenToWorld(new CanvasPoint(0, 0));
            var bottomRight = ScreenToWorld(new CanvasPoint(Width, Height));
            return new BoundingBox2D(topLeft.X, bottomRight.Y, bottomRight.X, topLeft.Y);
        }
    }

    /// <summary>
    ///     Sets the screen position of the world origin.
    /// </summary>
    /// <param name="origin">The screen position; both coordinates must be finite.</param>
    /// <exception cref="ArgumentOutOfRangeException">A coordinate is not finite.</exception>
    public void SetOrigin(CanvasPoint origin)
    {
        if (!double.IsFinite(origin.X) || !double.IsFinite(origin.Y))
            throw new ArgumentOutOfRangeException(nameof(origin), origin, "The origin must be finite.");
        _originSet = true;
        Update(origin, ScaleX, ScaleY, Width, Height);
    }

    /// <summary>
    ///     Sets the scale.
    /// </summary>
    /// <param name="scaleX">The horizontal scale in pixels per unit, between <see cref="MinScale" /> and <see cref="MaxScale" />.</param>
    /// <param name="scaleY">The vertical scale in pixels per unit, between <see cref="MinScale" /> and <see cref="MaxScale" />.</param>
    /// <exception cref="ArgumentOutOfRangeException">A scale is out of range.</exception>
    public void SetScale(double scaleX, double scaleY)
    {
        ValidateScale(scaleX, nameof(scaleX));
        ValidateScale(scaleY, nameof(scaleY));
        Update(Origin, scaleX, scaleY, Width, Height);
    }

    /// <summary>
    ///     Sets the size of the visible area. Renderers call this before drawing.
    /// </summary>
    /// <param name="width">The width in pixels; must be finite and not negative.</param>
    /// <param name="height">The height in pixels; must be finite and not negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">A dimension is invalid.</exception>
    public void Resize(double width, double height)
    {
        if (!(width >= 0) || !double.IsFinite(width))
            throw new ArgumentOutOfRangeException(nameof(width), width, "The width must be finite and not negative.");
        if (!(height >= 0) || !double.IsFinite(height))
            throw new ArgumentOutOfRangeException(nameof(height), height, "The height must be finite and not negative.");

        var origin = _originSet
            ? Origin
            : new CanvasPoint(width / 2 + _panBeforeSizing.X, height / 2 + _panBeforeSizing.Y);
        if (!_sized && _panBeforeSizing != default)
            _originSet = true;
        _sized = true;
        Update(origin, ScaleX, ScaleY, width, height);
    }

    /// <summary>
    ///     Moves the visible area by the specified number of pixels.
    /// </summary>
    /// <param name="deltaX">The horizontal distance.</param>
    /// <param name="deltaY">The vertical distance.</param>
    public void Pan(double deltaX, double deltaY)
    {
        if (!double.IsFinite(deltaX) || !double.IsFinite(deltaY))
            return;
        if (!_sized && !_originSet)
            _panBeforeSizing = new CanvasPoint(_panBeforeSizing.X + deltaX, _panBeforeSizing.Y + deltaY);
        else
            _originSet = true;
        Update(new CanvasPoint(Origin.X + deltaX, Origin.Y + deltaY), ScaleX, ScaleY, Width, Height);
    }

    /// <summary>
    ///     Zooms by a factor while keeping the world point under <paramref name="anchor" /> fixed. The scale is clamped to
    ///     [<see cref="MinScale" />, <see cref="MaxScale" />].
    /// </summary>
    /// <param name="anchor">The screen position that stays fixed, e.g. the mouse position.</param>
    /// <param name="factor">The zoom factor; greater than 1 zooms in. Must be positive and finite.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="factor" /> is not positive and finite.</exception>
    public void ZoomAt(CanvasPoint anchor, double factor)
    {
        if (!(factor > 0) || !double.IsFinite(factor))
            throw new ArgumentOutOfRangeException(nameof(factor), factor, "The zoom factor must be positive and finite.");

        var scaleX = Math.Clamp(ScaleX * factor, MinScale, MaxScale);
        var scaleY = Math.Clamp(ScaleY * factor, MinScale, MaxScale);
        if (!_sized && !_originSet)
        {
            // Without a size the anchor has no meaning yet; only the scale changes.
            Update(Origin, scaleX, scaleY, Width, Height);
            return;
        }

        var factorX = scaleX / ScaleX;
        var factorY = scaleY / ScaleY;
        var origin = new CanvasPoint(anchor.X - (anchor.X - Origin.X) * factorX,
            anchor.Y - (anchor.Y - Origin.Y) * factorY);
        _originSet = true;
        Update(origin, scaleX, scaleY, Width, Height);
    }

    /// <summary>
    ///     Converts a world position to screen coordinates.
    /// </summary>
    /// <param name="point">The world position.</param>
    /// <returns>The screen position.</returns>
    public CanvasPoint WorldToScreen(Point2D point) =>
        new(Origin.X + point.X * ScaleX, Origin.Y - point.Y * ScaleY);

    /// <summary>
    ///     Converts a screen position to world coordinates.
    /// </summary>
    /// <param name="point">The screen position.</param>
    /// <returns>The world position.</returns>
    public Point2D ScreenToWorld(CanvasPoint point) =>
        new((point.X - Origin.X) / ScaleX, (Origin.Y - point.Y) / ScaleY);

    private static void ValidateScale(double scale, string parameterName)
    {
        if (!(scale >= MinScale && scale <= MaxScale))
            throw new ArgumentOutOfRangeException(parameterName, scale,
                $"The scale must be between {MinScale} and {MaxScale} pixels per unit.");
    }

    private void Update(CanvasPoint origin, double scaleX, double scaleY, double width, double height)
    {
        if (origin == Origin && scaleX == ScaleX && scaleY == ScaleY && width == Width && height == Height)
            return;

        Origin = origin;
        ScaleX = scaleX;
        ScaleY = scaleY;
        Width = width;
        Height = height;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}
