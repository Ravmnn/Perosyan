using System;


namespace Psyan.Analyzer.Structures;




public class Conjunction(
    SyntacticStructure left,
    SyntacticStructure right,
    SyntacticStructure conjunction,
    SyntacticStructure conjunctionSplitter,
    SyntacticStructure conjunctionLinkSplitter
) : SyntacticStructure
{
    public SyntacticStructure Left { get; } = left;
    public SyntacticStructure Right { get; } = right;
    public SyntacticStructure ConjunctionWord { get; } = conjunction;
    public SyntacticStructure ConjunctionSplitter { get; } = conjunctionSplitter;
    public SyntacticStructure ConjunctionLinkSplitter { get; } = conjunctionLinkSplitter;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConjunction(this);


    public override Word BaseWord()
        => ConjunctionWord.BaseWord();
}
