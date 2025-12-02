namespace Psyan.Analyzer.Structures;




public class Conditional(
    SyntacticStructure ifParticle,
    SyntacticStructure conjunctionSentence
) : SyntacticStructure
{
    public SyntacticStructure IfParticle { get; } = ifParticle;
    public SyntacticStructure ConjunctionSentence { get; } = conjunctionSentence;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConditional(this);


    public override Word BaseWord()
        => IfParticle.BaseWord();
}
