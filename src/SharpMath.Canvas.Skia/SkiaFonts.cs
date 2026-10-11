using SkiaSharp;

namespace SharpMath.Canvas.Skia;

/// <summary>
///     Creates and caches Skia fonts and measures text. Not thread-safe; use one instance per thread.
/// </summary>
internal sealed class SkiaFonts : IDisposable
{
    private readonly Dictionary<FontSpec, SKFont> _fonts = [];
    private readonly Dictionary<string, SKTypeface> _typefaces = [];

    public SKFont Get(FontSpec font)
    {
        if (_fonts.TryGetValue(font, out var skFont))
            return skFont;

        if (!_typefaces.TryGetValue(font.Family, out var typeface))
        {
            typeface = SKTypeface.FromFamilyName(font.Family) ?? SKTypeface.CreateDefault();
            _typefaces.Add(font.Family, typeface);
        }

        skFont = new SKFont(typeface, (float) font.Size) { Subpixel = true };
        _fonts.Add(font, skFont);
        return skFont;
    }

    public TextSize Measure(string text, FontSpec font)
    {
        ArgumentNullException.ThrowIfNull(text);
        var skFont = Get(font);
        return new TextSize(skFont.MeasureText(text), skFont.Spacing);
    }

    public void Dispose()
    {
        foreach (var font in _fonts.Values)
            font.Dispose();
        foreach (var typeface in _typefaces.Values)
            typeface.Dispose();
        _fonts.Clear();
        _typefaces.Clear();
    }
}
