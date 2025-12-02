namespace Psyan.Analyzer.Structures;




public interface ISyntacticStructureProcessor
{
    void ProcessProbability(Probability probability);
    void ProcessConditional(Conditional conditional);
    void ProcessConditionalParticle(ConditionalParticle particle);
    void ProcessConjunction(Conjunction conjunction);
    void ProcessConjunctionParticle(ConjunctionParticle particle);
    void ProcessVerb(Verb verb);
    void ProcessVerbParticle(VerbParticle particle);
    void ProcessPreposition(Preposition preposition);
    void ProcessPrepositionParticle(PrepositionParticle particle);
    void ProcessPronoun(Pronoun pronoun);
    void ProcessNoun(Noun noun);
    void ProcessBoolean(Boolean boolean);
    void ProcessPunctuationParticle(PunctuationParticle particle);
    void ProcessExpect(Expect expect);
}




public abstract class SyntacticStructure
{
    public abstract void Process(ISyntacticStructureProcessor processor);

    public abstract Word BaseWord();
}
