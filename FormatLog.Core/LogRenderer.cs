using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Lithium.FormatLog;

public class LogRenderer
{
    public static readonly Regex FormattingExpression = new Regex(@"\{((?<PropertyName>\w+)(:{1}(?<Format>[^}]+))*)\}");
    public static readonly Regex LogLineExpression = new Regex(@"\$\{(?<Placeholder>@{0,1}[\w\d]+)\}");

    public async Task<string> RenderLineAsync(string line, string? format = null)
    {
        return await RenderLineAsync(line, format, CultureInfo.CurrentCulture);
    }

    public async Task<string> RenderLineAsync(string line, string? format, CultureInfo cultureInfo)
    {
        var log = JsonDocument.Parse(line);
        var timestamp = log.RootElement.GetProperty("@t").GetDateTime();
        var messageTemplate = log.RootElement.GetProperty("@mt").GetString();
        var formattedMessage = FormattingExpression.Replace(messageTemplate, match =>
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
        if (format == null)
        {
            return formattedMessage;
        }
        return LogLineExpression.Replace(format, match =>
        {
            var propertyName = match.Groups["Placeholder"].Value;
            if (propertyName == "FormattedMessage")
            {
                return formattedMessage;
            }
            else
            {
                if (log.RootElement.TryGetProperty(propertyName, out var property))
                {
                    return property.ValueKind switch
                    {
                        JsonValueKind.Number => property.GetDecimal().ToString(format, cultureInfo),
                        JsonValueKind.True => property.GetBoolean().ToString(),
                        JsonValueKind.False => property.GetBoolean().ToString(),
                        JsonValueKind.Null => "NULL",
                        _ => property.ToString()
                    } ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        });
    }

    record Log(DateTime @t, string @mt);
}
