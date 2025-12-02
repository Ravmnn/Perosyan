using System;


namespace Psyan.Analyzer.Structures;




public class Verb(
    SyntacticStructure? subject,
    SyntacticStructure moodParticle,
    SyntacticStructure? verbNoun,
    SyntacticStructure? tenseParticle = null,
    SyntacticStructure? aspectParticle = null,
    SyntacticStructure? destinationParticle = null,
    SyntacticStructure? destination = null,
    SyntacticStructure? objectParticle = null,
    SyntacticStructure? @object = null
) : SyntacticStructure
{
    public SyntacticStructure? Subject { get; } = subject;
    public SyntacticStructure MoodParticle { get; } = moodParticle;
    public SyntacticStructure? VerbNoun { get; } = verbNoun;
    public SyntacticStructure? TenseParticle { get; } = tenseParticle;
    public SyntacticStructure? AspectParticle { get; } = aspectParticle;

    public MoodType Mood { get; } = MoodTypeOf(moodParticle.BaseWord()) ?? throw new ArgumentException("Invalid mood word");
    public TenseType Tense { get; } = TenseTypeOf(tenseParticle?.BaseWord()) ?? throw new ArgumentException("Invalid tense word");
    public AspectType? Aspect { get; } = aspectParticle is null ? null : AspectTypeOf(aspectParticle.BaseWord());

    public SyntacticStructure? DestinationParticle { get; } = destinationParticle;
    public SyntacticStructure? Destination { get; } = destination;

    public SyntacticStructure? ObjectParticle { get; } = objectParticle;
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




    public enum AspectType
    {
        Continuous,
        Initiative,
        Terminative
    }


    public static AspectType? AspectTypeOf(Word word) => word.String switch
    {
        "na" => AspectType.Continuous,
        "te" => AspectType.Initiative,
        "ge" => AspectType.Terminative,

        _ => null
    };


    public static bool IsAspect(Word word)
        => AspectTypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessVerb(this);


    public override Word BaseWord()
        => MoodParticle.BaseWord();
}
