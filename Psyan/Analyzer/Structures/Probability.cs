namespace Psyan.Analyzer.Structures;




public class Probability(
    SyntacticStructure ifParticle,
    SyntacticStructure soParticle,
    SyntacticStructure splitter,
    SyntacticStructure sentence
) : SyntacticStructure
{
    public SyntacticStructure IfParticle { get; } = ifParticle;
    public SyntacticStructure SoParticle { get; } = soParticle;
    public SyntacticStructure Splitter { get; } = splitter;
    public SyntacticStructure Sentence { get; } = sentence;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessProbability(this);


    public override Word BaseWord()
        => IfParticle.BaseWord();
}
