using System;


namespace Psyan.Analyzer.Structures;




public class Conjunction(SyntacticStructure left, SyntacticStructure right, Word conjunction, Word conjunctionSplitter, Word conjunctionLinkSplitter)
    : SyntacticStructure
{
    public SyntacticStructure Left { get; } = left;
    public SyntacticStructure Right { get; } = right;
    public Word ConjunctionWord { get; } = conjunction;
    public Word ConjunctionSplitter { get; } = conjunctionSplitter;
    public Word ConjunctionLinkSplitter { get; } = conjunctionLinkSplitter;

    public LinkType Link { get; } = LinkTypeOf(conjunction) ?? throw new ArgumentException("Invalid conjunction word");




    public enum LinkType
    {
        CausalForward,
        CausalBackward,
        Adversative,
        Sequential
    }


    public static LinkType? LinkTypeOf(Word word) => word.String switch
    {
        "so" => LinkType.CausalForward,
        "ko" => LinkType.CausalBackward,
        "ma" => LinkType.Adversative,
        "ta" => LinkType.Sequential,

        _ => null
    };


    public static bool IsConjunction(Word word)
        => LinkTypeOf(word) is not null;


    public static bool IsConjunctionSplitter(Word word)
        => word.String == ";";


    public static bool IsConjunctionLinkSplitter(Word word)
        => word.String == ",";




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConjunction(this);


    public override Word BaseWord()
        => ConjunctionWord;
}
