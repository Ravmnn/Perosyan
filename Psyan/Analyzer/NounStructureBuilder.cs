namespace Psyan.Analyzer;




public class NounStructureBuilder : ISyntacticStructureBuilder
{
    public SyntacticStructure Build(SyntacticAnalyzer analyzer)
    {
        var noun = analyzer.Advance();
        var nextWord = analyzer.Peek();

        Word? adjective = IsNoun(nextWord) ? nextWord : null;

        if (adjective is not null)
            analyzer.Advance();

        return new NounStructure(noun, adjective);
    }


    public bool Match(SyntacticAnalyzer analyzer)
        => IsNoun(analyzer.Peek());


    private bool IsNoun(Word word)
        => word.Syllables.Length >= 2;
}
