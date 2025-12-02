namespace Psyan.Analyzer.Structures;




public interface ISyntacticStructureProcessor
{
    void ProcessConjunction(Conjunction conjunction);
    void ProcessVerb(Verb verb);
    void ProcessVerbParticle(VerbParticle particle);
    void ProcessPronoun(Pronoun pronoun);
    void ProcessNoun(Noun noun);
    void ProcessBoolean(Boolean boolean);
    void ProcessExpect(Expect expect);
}




public abstract class SyntacticStructure
{
    public abstract void Process(ISyntacticStructureProcessor processor);

    public abstract Word BaseWord();
}
