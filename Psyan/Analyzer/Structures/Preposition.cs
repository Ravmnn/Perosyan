namespace Psyan.Analyzer.Structures;




public class Preposition(SyntacticStructure left, SyntacticStructure right, SyntacticStructure prepositionParticle)
    : SyntacticStructure
{
    public SyntacticStructure Left { get; } = left;
    public SyntacticStructure Right { get; } = right;
    public SyntacticStructure PrepositionParticle { get; } = prepositionParticle;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessPreposition(this);


    public override Word BaseWord()
        => PrepositionParticle.BaseWord();
}
