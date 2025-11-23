using Psyan.Analyzer;


namespace Psyan;




public static class ErrorFormatter
{
    public static string FormatError(this InvalidSyllable error)
    {
        var word = error.Word;
        word = word.Insert(error.Start + 1, "[/]");
        word = word.Insert(error.Start, "[underline red]");

        return $"{error.ErrorMessage}, at word [green]\"{word}\"[/].";
    }
}
