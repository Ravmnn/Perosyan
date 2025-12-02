namespace Psyan.Analyzer.Structures;




public class Noun(Word word, Word? adjective = null) : SyntacticStructure
{
    public Word Word { get; } = word;
    public Word? Adjective { get; } = adjective;




    public static bool IsNoun(Word word)
        => word.Syllables.Length >= 2 && !word.HasError;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessNoun(this);


    public override Word BaseWord()
        => Word;
}
