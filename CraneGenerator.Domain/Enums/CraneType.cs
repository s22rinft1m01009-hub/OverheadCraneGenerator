namespace CraneGenerator.Domain.Enums;

public enum CraneType
{
    SingleGirderEot = 1,
    DoubleGirderEot = 2,
    Gantry = 3
}

public static class CraneTypeNormalizer
{
    public const string SingleGirderEot = "SINGLE_GIRDER_EOT";
    public const string DoubleGirderEot = "DOUBLE_GIRDER_EOT";
    public const string Gantry = "GANTRY";

    public static string Normalize(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            SingleGirderEot or "SINGLE GIRDER EOT" or "EOT CRANE" or "EOT" => SingleGirderEot,
            DoubleGirderEot or "DOUBLE GIRDER EOT" => DoubleGirderEot,
            Gantry or "GANTRY CRANE" => Gantry,
            _ => throw new ArgumentException($"Unsupported crane type '{value}'.")
        };
    }

    public static CraneType Parse(string? value)
    {
        return Normalize(value) switch
        {
            SingleGirderEot => CraneType.SingleGirderEot,
            DoubleGirderEot => CraneType.DoubleGirderEot,
            Gantry => CraneType.Gantry,
            _ => throw new ArgumentException($"Unsupported crane type '{value}'.")
        };
    }
}
