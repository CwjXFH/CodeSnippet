using System.Text;

namespace EC.Protobuf.Formatters;

internal static class InternalEncoding
{
    private static readonly Encoding _utf8NoBOM = new UTF8Encoding(false, true);

    public static Encoding UTF8WithBOM => Encoding.UTF8;

    public static Encoding UTF8NoBOM => _utf8NoBOM;
}