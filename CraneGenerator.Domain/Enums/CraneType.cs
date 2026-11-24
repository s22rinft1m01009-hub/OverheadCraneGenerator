namespace CraneGenerator.Domain.Enums;

public enum CraneType
{
    SingleGirderEot = 1,
    DoubleGirderEot = 2,
    Gantry = 3,
    JibCrane = 4,
    Monorail = 5,
    WallMountedJib = 6,
    PortableGantry = 7,
    SemiGantry = 8,
    Underslung = 9
}

public static class CraneTypeNormalizer
{
    public const string SingleGirderEot = "SINGLE_GIRDER_EOT";
    public const string DoubleGirderEot = "DOUBLE_GIRDER_EOT";
    public const string Gantry = "GANTRY";
    public const string JibCrane = "JIB_CRANE";
    public const string Monorail = "MONORAIL";
    public const string WallMountedJib = "WALL_MOUNTED_JIB";
    public const string PortableGantry = "PORTABLE_GANTRY";
    public const string SemiGantry = "SEMI_GANTRY";
    public const string Underslung = "UNDERSLUNG";

    public static string Normalize(string? value)
    {
        return value?.Trim().ToUpperInvariant() switch
        {
            SingleGirderEot or "SINGLE GIRDER EOT" or "EOT CRANE" or "EOT" => SingleGirderEot,
            DoubleGirderEot or "DOUBLE GIRDER EOT" => DoubleGirderEot,
            Gantry or "GANTRY CRANE" => Gantry,
            JibCrane or "JIB" => JibCrane,
            Monorail or "MONORAIL CRANE" => Monorail,
            WallMountedJib or "WALL MOUNTED JIB" or "WALL JIB" => WallMountedJib,
            PortableGantry or "PORTABLE GANTRY CRANE" => PortableGantry,
            SemiGantry or "SEMI GANTRY" or "SEMI-GANTRY" => SemiGantry,
            Underslung or "UNDERSLUNG CRANE" => Underslung,
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
            JibCrane => CraneType.JibCrane,
            Monorail => CraneType.Monorail,
            WallMountedJib => CraneType.WallMountedJib,
            PortableGantry => CraneType.PortableGantry,
            SemiGantry => CraneType.SemiGantry,
            Underslung => CraneType.Underslung,
            _ => throw new ArgumentException($"Unsupported crane type '{value}'.")
        };
    }
}
