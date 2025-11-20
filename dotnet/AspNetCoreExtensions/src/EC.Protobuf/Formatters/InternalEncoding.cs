using System.Text;

namespace EC.Protobuf.Formatters;

internal static class InternalEncoding
{
    public static Encoding UTF8WithBOM => Encoding.UTF8;

    public static Encoding UTF8NoBOM { get; } = new UTF8Encoding(false, true);
}
