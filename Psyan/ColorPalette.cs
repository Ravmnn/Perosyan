using Spectre.Console;


namespace Psyan;




public static class ColorPalette
{
    public static Style Invalid => Color.Red3;
    public static Style InvalidOrthography => Color.Red3_1;

    public static Style PunctuationParticle => Color.Gray62;

    public static Style Boolean => Color.LightSlateBlue;
    public static Style Noun => Color.SteelBlue1;
    public static Style Adjective => Color.SteelBlue1_1;
    public static Style Pronoun => Color.Gold3_1;

    public static Style PrepositionParticle => Color.IndianRed_1;

    public static Style VerbMoodParticle => Color.DarkOliveGreen2;
    public static Style VerbNoun => Color.SpringGreen2_1;
    public static Style VerbTenseParticle => Color.Violet;
    public static Style VerbAspectParticle => Color.Plum2;
    public static Style VerbDestinationParticle => Color.Honeydew2;
    public static Style VerbObjectParticle => Color.Honeydew2;

    public static Style ConjunctionParticle => Color.HotPink_1;

    public static Style ConditionalParticle => Color.HotPink_1;
}
