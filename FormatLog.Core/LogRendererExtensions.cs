using System.Globalization;

namespace Lithium.FormatLog;

public static class LogRendererExtensions
{
    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync());

    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader, string format) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync(), format);

    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader, CultureInfo cultureInfo) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync(), null, cultureInfo);

    public static async Task<string> RenderLineAsync(this LogRenderer renderer, TextReader reader, string format, CultureInfo cultureInfo) =>
        await renderer.RenderLineAsync(await reader.ReadLineAsync(), format, cultureInfo);
}
