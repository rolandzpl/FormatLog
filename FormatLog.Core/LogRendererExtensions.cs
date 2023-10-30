using System.Globalization;

namespace Lithium.FormatLog;

public static class LogRendererExtensions
{
    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync());

    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader, CultureInfo cultureInfo) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync(), cultureInfo);
}
