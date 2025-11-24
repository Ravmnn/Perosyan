namespace Psyan.Analyzer;




public class PronounStructureBuilder : ISyntacticStructureBuilder
{
    public string[] Pronouns { get; } = ["mi", "tu", "le", "mis", "tus", "mis"];




    public SyntacticStructure Build(SyntacticAnalyzer analyzer)
    {
        var noun = analyzer.Advance();
        var nextWord = analyzer.Peek();

        Word? adjective = IsNoun(nextWord) ? nextWord : null;

        if (adjective is not null)
            analyzer.Advance();

        return new PronounStructure(noun, adjective);
    }


    public bool Match(SyntacticAnalyzer analyzer)
        => IsPronoun(analyzer.Peek());


    private bool IsPronoun(Word word)
        => Pronouns.Contains(word.String);

    private bool IsNoun(Word word)
        => word.Syllables.Length >= 2;
}
