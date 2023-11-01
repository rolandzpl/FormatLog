using System.Globalization;

namespace Lithium.FormatLog;

public class LogRendererTests
{
    [Test]
    public async Task RenderLineAsync_GivenLogReader_RendersLine()
    {
        using var reader = new StringReader(Logs.Example1);
        var formattedLog = await renderer.RenderLineAsync(reader);
        Assert.That(formattedLog, Is.Not.Null);
    }

    [Test]
    public async Task RenderLineAsync_GivenLogReaderAndFormat_RendersLineAccordingToFormat_1()
    {
        using var reader = new StringReader(Logs.Example1);
        var formattedLog = await renderer.RenderLineAsync(reader, "${@t} ${FormattedMessage}");
        Assert.That(formattedLog, Is.EqualTo("2023-10-26T04:45:30.5362270Z HTTP GET /css/site.css responded 304 in 0,1725 ms"));
    }

    [Test]
    public async Task RenderLineAsync_GivenLogReaderAndFormat_RendersLineAccordingToFormat_2()
    {
        using var reader = new StringReader(Logs.Example1);
        var formattedLog = await renderer.RenderLineAsync(reader, "${@t} ${ClientIp} ${SourceContext} ${FormattedMessage}");
        Assert.That(formattedLog, Is.EqualTo("2023-10-26T04:45:30.5362270Z 90.156.5.165 Serilog.AspNetCore.RequestLoggingMiddleware HTTP GET /css/site.css responded 304 in 0,1725 ms"));
    }

    [Test]
    public async Task RenderLineAsync_GivenLogReaderAndFormat_RendersLineAccordingToFormat_3()
    {
        using var reader = new StringReader(Logs.Example1);
        var formattedLog = await renderer.RenderLineAsync(reader, "${@t} ${NonExistingProperty} ${FormattedMessage}");
        Assert.That(formattedLog, Is.EqualTo("2023-10-26T04:45:30.5362270Z  HTTP GET /css/site.css responded 304 in 0,1725 ms"));
    }

    [Test]
    public async Task Loop_GivenLogReader_RendersToWriter()
    {
        using var reader = new StringReader(Logs.Example1);
        using var writer = new StringWriter();
        CancellationToken cancellationToken = default;
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line == null)
            {
                break;
            }
            if (line == string.Empty)
            {
                continue;
            }
            await writer.WriteLineAsync(await renderer.RenderLineAsync(line));
        }
        Assert.That(writer.ToString().LineCount(), Is.EqualTo(1));
    }

    [Test]
    public void Expression_GivenLogExampleWithFourPlaceholders_ReturnsFourCaptures()
    {
        var matches = LogRenderer.FormattingExpression.Matches(Logs.Example1);
        Assert.That(matches.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task RenderLineAsync_ProvidedStructuredLog_ReturnsLogTextRepresentation()
    {
        Assert.That(
            await renderer.RenderLineAsync(Logs.Example1, null, CultureInfo.InvariantCulture),
            Is.EqualTo(@"HTTP GET /css/site.css responded 304 in 0.1725 ms"));
    }

    [SetUp]
    protected async Task SetUp()
    {
        renderer = new LogRenderer();
    }

    private LogRenderer renderer;
}
