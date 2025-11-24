using Spectre.Console;

using PrettyPrompt;

using Psyan.Analyzer;


namespace Psyan;




class Program
{
    public static async Task Main(string[] args)
    {
        args = ["-s", "mi ni kome pa na"];

        var root = new PerosyanRootCommand(args);

        await root.Result.InvokeAsync();
    }




    public static async Task Run(PerosyanRootCommand root, PerosyanOptions options)
    {
        if (options.Interactive)
        {
            await RunInteractive(options);
            return;
        }

        if (options.Source is null && options.SourceFile is null)
        {
            var source = await Console.In.ReadToEndAsync();
            options = options with { Source = source };
        }

        RunPassive(options);
    }




    private static void RunPassive(PerosyanOptions options)
    {
        var source = options.Source ?? File.ReadAllText(options.SourceFile!);
        var tokens = new Lexer(source).Tokenize();
        var words = OrthographicAnalyzer.GetWords(tokens);

        var structures = new SyntacticAnalyzer(words).GetStructures();


    }




    public static async Task RunInteractive(PerosyanOptions options)
    {
        var prompt = new Prompt(callbacks: new PerosyanPromptCallbacks());

        var result = await prompt.ReadLineAsync();

        RunPassive(options with { Source = result.Text });
    }
}
