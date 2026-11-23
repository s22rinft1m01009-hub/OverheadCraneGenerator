using PdfSharp.Fonts;

namespace CraneGenerator.Application.Drawing.Pdf.Fonts;

/// <summary>
/// Resolves Arial requests using fonts installed on Windows.
/// </summary>
public sealed class CraneFontResolver : IFontResolver
{
    private const string RegularFace = "CraneGenerator.WindowsArial.Regular";
    private const string BoldFace = "CraneGenerator.WindowsArial.Bold";
    private static readonly object RegistrationLock = new();
    private static readonly Lazy<byte[]> RegularFont = new(() => LoadFont("arial.ttf"));
    private static readonly Lazy<byte[]> BoldFont = new(() => LoadFont("arialbd.ttf"));

    public string DefaultFontName => "Arial";

    /// <summary>
    /// Installs the resolver once, before PDFsharp creates any fonts.
    /// </summary>
    public static void Register()
    {
        lock (RegistrationLock)
        {
            if (GlobalFontSettings.FontResolver is CraneFontResolver)
                return;

            if (GlobalFontSettings.FontResolver is not null)
            {
                throw new InvalidOperationException(
                    "A different PDFsharp font resolver has already been configured.");
            }

            GlobalFontSettings.FontResolver = new CraneFontResolver();
        }
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (!familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase) &&
            !familyName.Equals("Liberation Sans", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var faceName = isBold ? BoldFace : RegularFace;
        return new FontResolverInfo(faceName, mustSimulateBold: false, mustSimulateItalic: isItalic);
    }

    public byte[] GetFont(string faceName) => faceName switch
    {
        RegularFace => RegularFont.Value,
        BoldFace => BoldFont.Value,
        _ => throw new ArgumentException($"Unknown font face '{faceName}'.", nameof(faceName))
    };

    private static byte[] LoadFont(string fileName)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new InvalidOperationException(
                "The PDF font resolver is configured to use Arial installed on Windows, but the current operating system is not Windows.");
        }

        var fontsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        var fontPath = Path.Combine(fontsDirectory, fileName);
        if (!File.Exists(fontPath))
        {
            throw new InvalidOperationException(
                $"Required Windows font '{fileName}' was not found at '{fontPath}'. Install Arial or switch to bundled font files.");
        }

        return File.ReadAllBytes(fontPath);
    }
}
