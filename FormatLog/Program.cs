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

var rootCommand = new RootCommand("Sample app for System.CommandLine");
rootCommand.SetHandler(async (reader, writer) =>
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
        await writer!.WriteLineAsync(await renderer.RenderLineAsync(line));
    }
}, inOption, outOption);

rootCommand.AddOption(inOption);
rootCommand.AddOption(outOption);

return await rootCommand.InvokeAsync(args);
