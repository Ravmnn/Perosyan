using Spectre.Console;


namespace Psyan;




public static class ColorPalette
{
    public static Style Invalid => Color.Red3;
    public static Style InvalidOrthography => Color.Red3_1;

    public static Style Punctuaction => Color.Gray62;

    public static Style Noun => Color.SteelBlue1;
    public static Style Adjective => Color.SteelBlue1_1;
    public static Style Pronoun => Color.Gold3_1;

    public static Style VerbMood => Color.DarkOliveGreen2;
    public static Style VerbNoun => Color.SpringGreen2_1;
    public static Style VerbTense => Color.Violet;
    public static Style VerbDestinationSpecifier => Color.Honeydew2;
    public static Style VerbObjectSpecifier => Color.Honeydew2;

    public static Style Conjunction => Color.HotPink_1;
}
