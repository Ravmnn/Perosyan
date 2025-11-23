using Spectre.Console;

using Psyan.Analyzer;


namespace Psyan;




public static class Log
{
    public static void Error(InvalidSyllable error)
        => AnsiConsole.MarkupLine(error.FormatError());
}
