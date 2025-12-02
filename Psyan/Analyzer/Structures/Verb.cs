using System;


namespace Psyan.Analyzer.Structures;




public class Verb(
    SyntacticStructure? subject,
    SyntacticStructure moodWord,
    SyntacticStructure? verbNoun,
    SyntacticStructure? tenseWord = null,
    SyntacticStructure? destinationSpecifier = null,
    SyntacticStructure? destination = null,
    SyntacticStructure? objectSpecifier = null,
    SyntacticStructure? @object = null
) : SyntacticStructure
{
    public SyntacticStructure? Subject { get; } = subject;
    public SyntacticStructure MoodWord { get; } = moodWord;
    public SyntacticStructure? VerbNoun { get; } = verbNoun;
    public SyntacticStructure? TenseWord { get; } = tenseWord;

    public MoodType Mood { get; } = MoodTypeOf(moodWord.BaseWord()) ?? throw new ArgumentException("Invalid mood word");
    public TenseType Tense { get; } = TenseTypeOf(tenseWord?.BaseWord()) ?? throw new ArgumentException("Invalid tense word");

    // TODO: add aspects

    public SyntacticStructure? DestinationSpecifier { get; } = destinationSpecifier;
    public SyntacticStructure? Destination { get; } = destination;

    public SyntacticStructure? ObjectSpecifier { get; } = objectSpecifier;
    public SyntacticStructure? Object { get; } = @object;




    public enum MoodType
    {
        Indicative,
        Questionative,
        Imperative
    }


    public static MoodType? MoodTypeOf(Word word) => word.Token.Lexeme switch
    {
        "ni" => MoodType.Indicative,
        "ka" => MoodType.Questionative,
        "go" => MoodType.Imperative,

        _ => null
    };


    public static bool IsMood(Word word)
        => MoodTypeOf(word) is not null;




    public enum TenseType
    {
        Past,
        Present,
        Future
    }


    public static TenseType? TenseTypeOf(Word? word) => word is null ? TenseType.Present : word.Value.Token.Lexeme switch
    {
        "pa" => TenseType.Past,
        "fu" => TenseType.Future,

        _ => null
    };


    public static bool IsTense(Word word)
        => TenseTypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessVerb(this);


    public override Word BaseWord()
        => MoodWord.BaseWord();
}
