namespace Psyan.Analyzer;




public class NounStructure(Word noun, Word? adjective = null)
    : SyntacticStructure(adjective is { } valid ? [noun, valid] : [noun])
{
    public Word Noun { get; } = noun;
    public Word? Adjective { get; } = adjective;
}
