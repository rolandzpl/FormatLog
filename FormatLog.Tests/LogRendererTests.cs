using System.Globalization;

namespace Lithium.CLEF;

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
        var matches = LogRenderer.Expression.Matches(Logs.Example1);
        Assert.That(matches.Count(), Is.EqualTo(4));
    }

    [Test]
    public async Task RenderLineAsync_ProvidedStructuredLog_ReturnsLogTextRepresentation()
    {
        Assert.That(
            await renderer.RenderLineAsync(Logs.Example1, CultureInfo.InvariantCulture),
            Is.EqualTo(@"HTTP GET /css/site.css responded 304 in 0.1725 ms"));
    }

    [SetUp]
    protected async Task SetUp()
    {
        renderer = new LogRenderer();
    }

    private LogRenderer renderer;
}
