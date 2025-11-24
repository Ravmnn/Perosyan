namespace Psyan.Analyzer;




public enum VerbMood
{
    Invalid,

    Indicative,
    Question,
    Imperative
}


public enum VerbTense
{
    Invalid,

    Present,
    Past,
    Future
}




public static class VerbalParticlesExtensions
{
    public static string VerbMoodToString(this VerbMood mood) => mood switch
    {
        VerbMood.Indicative => "ni",
        VerbMood.Question => "ka",
        VerbMood.Imperative => "go",

        _ => string.Empty
    };


    public static VerbMood StringToVerbMood(this string mood) => mood switch
    {
         "ni" => VerbMood.Indicative,
         "ka" => VerbMood.Question,
         "go" => VerbMood.Imperative,

        _ => VerbMood.Invalid
    };




    public static string VerbTenseToString(this VerbTense tense) => tense switch
    {
        VerbTense.Present => "",
        VerbTense.Past => "pa",
        VerbTense.Future => "fu",

        _ => string.Empty
    };


    public static VerbTense StringToVerbTense(this string tense) => tense switch
    {
         "" => VerbTense.Present,
         "pa" => VerbTense.Past,
         "fu" => VerbTense.Future,

        _ => VerbTense.Invalid
    };
}
