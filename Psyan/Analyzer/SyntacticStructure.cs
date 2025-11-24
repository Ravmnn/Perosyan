namespace Psyan.Analyzer;




public abstract class SyntacticStructure(Word[] words)
{
    public virtual Word[] Words { get; } = words;

    public Word Start => Words.First();
    public Word End => Words.Last();
}




public class InvalidSyntacticStructure(Word[] words, string errorMessage)
    : SyntacticStructure(words)
{
    public string ErrorMessage { get; } = errorMessage;
}
