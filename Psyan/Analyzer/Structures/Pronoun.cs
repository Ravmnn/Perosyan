using System;


namespace Psyan.Analyzer.Structures;




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
