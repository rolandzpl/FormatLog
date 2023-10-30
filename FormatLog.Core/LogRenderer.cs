using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lithium.FormatLog;

public class LogRenderer
{
    public static readonly Regex Expression = new Regex(@"\{((?<PropertyName>\w+)(:{1}(?<Format>[^}]+))*)\}");

    public async Task<string> RenderLineAsync(string? line)
    {
        return await RenderLineAsync(line, CultureInfo.CurrentCulture);
    }

    public async Task<string> RenderLineAsync(string? line, CultureInfo cultureInfo)
    {
        var log = JsonDocument.Parse(line);
        var timestamp = log.RootElement.GetProperty("@t").GetDateTime();
        var messageTemplate = log.RootElement.GetProperty("@mt").GetString();
        return Expression.Replace(messageTemplate, match =>
        {
            var propertyName = match.Groups["PropertyName"].Value;
            var format = match.Groups["Format"].Value.TrimStart(':');
            var property = log.RootElement.GetProperty(propertyName);
            var replacedString = property.ValueKind switch
            {
                JsonValueKind.Number => property.GetDecimal().ToString(format, cultureInfo),
                JsonValueKind.True => property.GetBoolean().ToString(),
                JsonValueKind.False => property.GetBoolean().ToString(),
                JsonValueKind.Null => "NULL",
                _ => property.ToString()
            };
            return replacedString ?? string.Empty;
        });
    }

    record Log(DateTime @t, string @mt);
}
