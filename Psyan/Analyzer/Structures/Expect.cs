namespace Psyan.Analyzer.Structures;




public class Expect(Word? after, Word? got, string message, bool atEnd = false) : SyntacticStructure
{
    public Word? After { get; } = after;
    public Word? Got { get; } = got;

    public string Message { get; } = message;
    public bool AtEnd { get; } = atEnd;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessExpect(this);
}
