namespace Psyan.Analyzer;




public class SyntacticAnalyzer(IEnumerable<Token> tokens)
{
    private uint _current;




    public string[] Splitters { get; } = ["!", "?", "."];


    public Token[] Tokens { get; set; } = tokens.ToArray();

    public List<ISyntacticStructureBuilder> Builders { get; set; } = [];




    public IEnumerable<SyntacticStructure> Analyze()
    {
        var structures = new List<SyntacticStructure>();

        while (!AtEnd())
        {
            foreach (var builder in Builders)
                if (builder.Match(this))
                    structures.Add(builder.Build(this));

            var start = Peek();
            AdvanceUntilSplitter();
            var end = Peek();
        }

        return structures;
    }


    private void AdvanceUntilSplitter()
    {
        while (Peek() is var token && !Splitters.Contains(token.Lexeme))
            Advance();
    }




    public Token Advance()
    {
        if (AtEnd())
            return Previous();

        return Tokens[_current++];
    }


    public Token Peek()
        => !AtEnd() ? Tokens[_current] : Previous();


    public Token Previous()
        => Tokens[_current - 1];


    public Token Next()
    {
        if (_current + 1 >= Tokens.Length)
            return Peek();

        return Tokens[_current + 1];
    }


    public bool AtEnd()
        => _current >= Tokens.Length;
}
