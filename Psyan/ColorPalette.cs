using PrettyPrompt.Highlighting;

using Spectre.Console;


namespace Psyan;




public static class ColorPalette
{
    public static AnsiColor Invalid { get; } = ToAnsiColor(Color.Red3);
    public static AnsiColor InvalidOrthography { get; } = ToAnsiColor(Color.Red3_1);

    public static AnsiColor Punctuaction { get; } = ToAnsiColor(Color.Gray62);

    public static AnsiColor Noun { get; } = ToAnsiColor(Color.SteelBlue1);
    public static AnsiColor Adjective { get; } = ToAnsiColor(Color.SteelBlue1_1);
    public static AnsiColor Pronoun { get; } = ToAnsiColor(Color.Gold3_1);

    public static AnsiColor VerbMood { get; } = ToAnsiColor(Color.DarkOliveGreen2);
    public static AnsiColor VerbNoun { get; } = ToAnsiColor(Color.SpringGreen2_1);
    public static AnsiColor VerbTense { get; } = ToAnsiColor(Color.Violet);
    public static AnsiColor VerbDestinationSpecifier { get; } = ToAnsiColor(Color.Honeydew2);
    public static AnsiColor VerbObjectSpecifier { get; } = ToAnsiColor(Color.Honeydew2);

    public static AnsiColor Conjunction { get; } = ToAnsiColor(Color.HotPink_1);




    public static AnsiColor ToAnsiColor(Color color)
        => AnsiColor.Rgb(color.R, color.G, color.B);
}
