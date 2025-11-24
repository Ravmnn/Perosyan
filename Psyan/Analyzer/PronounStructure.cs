namespace Psyan.Analyzer;




public class PronounStructure(Word pronoun, Word? adjective = null)
    : SyntacticStructure(adjective is { } valid ? [pronoun, valid] : [pronoun])
{
    public Word Pronoun { get; } = pronoun;
    public Word? Adjective { get; } = adjective;
}
