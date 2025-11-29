namespace Psyan.Analyzer;




public interface ISyntacticStructureProcessor
{
    void ProcessNoun(Noun noun);
    void ProcessPronoun(Pronoun pronoun);
    void ProcessVerb(Verb verb);
    void ProcessInvalid(Invalid invalid);
}




public abstract class SyntacticStructure
{
    public abstract void Process(ISyntacticStructureProcessor processor);
}




public class Verb(SyntacticStructure? subject, Word moodWord, Word verbNounWord, Word? tenseWord = null,
    Word? destinationSpecifier = null, SyntacticStructure? destination = null, Word? objectSpecifier = null, SyntacticStructure? @object = null)
    : SyntacticStructure
{
    public SyntacticStructure? Subject { get; } = subject;
    public Word MoodWord { get; } = moodWord;
    public Word VerbNounWord { get; } = verbNounWord;
    public Word? TenseWord { get; } = tenseWord;

    public MoodType Mood { get; } = MoodTypeOf(moodWord) ?? throw new ArgumentException("Invalid mood word");
    public TenseType Tense { get; } = TenseTypeOf(tenseWord) ?? throw new ArgumentException("Invalid tense word");

    // TODO: add aspects
    // TODO: add conjunctions (+ punctuation)

    public Word? DestinationSpecifier { get; } = destinationSpecifier;
    public SyntacticStructure? Destination { get; } = destination;

    public Word? ObjectSpecifier { get; } = objectSpecifier;
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
}




public class Pronoun(Word pronoun) : SyntacticStructure
{
    public Word Word { get; } = pronoun;
    public PersonType Person { get; } = PersonTypeOf(pronoun) ?? throw new ArgumentException("Invalid pronoun word");




    public enum PersonType
    {
        First,
        Second,
        Third
    }


    public static PersonType? PersonTypeOf(Word word) => word.Token.Lexeme switch
    {
        "mi" or "mis" => PersonType.First,
        "tu" or "tus" => PersonType.Second,
        "le" or "les" => PersonType.Third,

        _ => null
    };




    public static bool IsPronoun(Word word)
        => PersonTypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessPronoun(this);
}




public class Noun(Word word, Word? adjective = null) : SyntacticStructure
{
    public Word Word { get; } = word;
    public Word? Adjective { get; } = adjective;




    public static bool IsNoun(Word word)
        => word.Syllables.Length >= 2 && !word.HasError;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessNoun(this);
}




public class Invalid(Word word, string message) : SyntacticStructure
{
    public Word Word { get; } = word;
    public string Message { get; } = message;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessInvalid(this);
}
