using PrettyPrompt;

using Spectre.Console.Cli;

using Psyan.Analyzer;


namespace Psyan;




public class PisanCommandSettings : CommandSettings
{
    [CommandOption("-s|--source")]
    public string? Source { get; init; }

    [CommandOption("-f|--file")]
    public string? SourceFile { get; init; }


    [CommandOption("-i|--interactive")]
    public bool Interactive { get; init; }
}




public class PisanCommand : Command<PisanCommandSettings>
{
    protected override int Execute(CommandContext context, PisanCommandSettings settings, CancellationToken cancellationToken)
    {
        if (settings.Interactive)
        {
            RunInteractive(settings).Wait(cancellationToken);
            return 0;
        }

        var source = string.Empty;

        if (settings.Source is null && settings.SourceFile is null)
            source = Console.In.ReadToEndAsync(cancellationToken).Result;

        RunPassive(source, settings);
        return 0;
    }




    private static void RunPassive(string source, PisanCommandSettings settings)
    {
        var tokens = new Lexer(source).Tokenize();
        var words = OrthographicAnalyzer.GetWords(tokens);

        var structures = new Parser(words.ToArray()).Parse();

        // TODO: once you got a minimal usable version, add ability to print the whole analysis data as JSON to the stdout:
        // Passive mode will be the method that outputs that JSON to stdout
    }




    public static async Task RunInteractive(PisanCommandSettings settings)
    {
        var prompt = new Prompt(callbacks: new PisanPromptCallbacks());

        var result = await prompt.ReadLineAsync();

        RunPassive(result.Text, settings);
    }
}
