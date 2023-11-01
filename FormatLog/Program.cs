using System.CommandLine;
using Lithium.FormatLog;

var inOption = new Option<TextReader?>(
    name: "--in",
    description: "Input of the log lines",
    parseArgument: result =>
    {
        if (result.Tokens.Count != 1)
        {
            result.ErrorMessage = "--in requires file path";
            return null;
        }
        return new StreamReader(File.OpenRead(result.Tokens.First().Value));
    }
);
inOption.AddAlias("-i");
inOption.SetDefaultValue(Console.In);

var outOption = new Option<TextWriter?>(
    name: "--out",
    description: "Output where the log lines are to be renderred",
    parseArgument: result =>
    {
        if (result.Tokens.Count != 1)
        {
            result.ErrorMessage = "--out requires file path";
            return null;
        }
        return new StreamWriter(File.OpenWrite(result.Tokens.First().Value));
    }
);
outOption.AddAlias("-o");
outOption.SetDefaultValue(Console.Out);

var formatOption=new Option<string >(
    name:"--format",
    description:"Line format"
);
formatOption.AddAlias("-f");
formatOption.SetDefaultValue("${@t} ${ClientIp} ${SourceContext} ${FormattedMessage}");

var rootCommand = new RootCommand("Structured logs command line formatting tool");
rootCommand.SetHandler(async (reader, writer, format) =>
{
    var renderer = new LogRenderer();

    CancellationToken cancellationToken = default;

    while (!cancellationToken.IsCancellationRequested)
    {
        var line = await reader!.ReadLineAsync(cancellationToken);
        if (line == null)
        {
            break;
        }
        if (line == string.Empty)
        {
            continue;
        }
        await writer!.WriteLineAsync(await renderer.RenderLineAsync(line, format));
    }
}, inOption, outOption, formatOption);

rootCommand.AddOption(inOption);
rootCommand.AddOption(outOption);
rootCommand.AddOption(formatOption);

return await rootCommand.InvokeAsync(args);
