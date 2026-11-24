using PdfSharp.Fonts;
using System;
using System.IO;
using System.Reflection;

namespace CraneGenerator.Application.Drawing.Pdf.Fonts;

/// <summary>
/// Resolves font requests using bundled embedded Liberation Sans TTF resources.
/// Embedded as EmbeddedResource in CraneGenerator.Application.csproj.
/// </summary>
public sealed class CraneFontResolver : IFontResolver
{
    private const string RegularFace = "CraneGenerator.LiberationSans.Regular";
    private const string BoldFace = "CraneGenerator.LiberationSans.Bold";
    private static readonly object RegistrationLock = new();
    private static readonly Lazy<byte[]> RegularFont = new(() => LoadEmbeddedFont("LiberationSans-Regular.ttf"));
    private static readonly Lazy<byte[]> BoldFont = new(() => LoadEmbeddedFont("LiberationSans-Bold.ttf"));

    public string DefaultFontName => "Liberation Sans";

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
                return;

            GlobalFontSettings.FontResolver = new CraneFontResolver();
        }
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var lower = (familyName ?? string.Empty).ToLowerInvariant();
        var bold = isBold || lower.Contains("bold");
        var faceName = bold ? BoldFace : RegularFace;

        return new FontResolverInfo(faceName, mustSimulateBold: false, mustSimulateItalic: isItalic);
    }

    public byte[] GetFont(string faceName) => faceName switch
    {
        RegularFace => RegularFont.Value,
        BoldFace => BoldFont.Value,
        _ => RegularFont.Value
    };

    private static byte[] LoadEmbeddedFont(string fileName)
    {
        var assembly = typeof(CraneFontResolver).Assembly;
        var resourceName = $"CraneGenerator.Application.Drawing.Pdf.Fonts.Assets.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException(
                $"Embedded font resource '{resourceName}' was not found. " +
                "Ensure Liberation Sans TTF files are added to the project as EmbeddedResource.");
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }
}
