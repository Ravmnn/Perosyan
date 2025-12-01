namespace Psyan.Analyzer.Structures;




public interface ISyntacticStructureProcessor
{
    void ProcessConjunction(Conjunction conjunction);
    void ProcessVerb(Verb verb);
    void ProcessPronoun(Pronoun pronoun);
    void ProcessNoun(Noun noun);
    void ProcessExpect(Expect expect);
}




public abstract class SyntacticStructure
{
    public abstract void Process(ISyntacticStructureProcessor processor);
}
