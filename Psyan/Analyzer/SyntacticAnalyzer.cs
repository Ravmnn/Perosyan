namespace Psyan.Analyzer;




public class SyntacticAnalyzer(IEnumerable<Word> words)
{
    private uint _current;




    public string[] Splitters { get; } = ["!", "?", "."];


    public Word[] Words { get; set; } = words.ToArray();


    private readonly List<SyntacticStructure> _structures = [];

    public List<ISyntacticStructureBuilder> Builders { get; set; } = [
        new NounStructureBuilder(),
        new PronounStructureBuilder(),
        new VerbalStructureBuilder()
    ];




    public IEnumerable<SyntacticStructure> GetStructures()
    {
        _structures.Clear();

        while (!AtEnd())
        {
            var success = false;

            foreach (var builder in Builders)
                if (builder.Match(this))
                {
                    _structures.Add(builder.Build(this));
                    success = true;
                    break;
                }

            if (!success)
                AddInvalidStructure(_structures);
        }

        return _structures;
    }


    private void AddInvalidStructure(List<SyntacticStructure> structures)
        => structures.Add(new InvalidSyntacticStructure(AdvanceSentence(), "Invalid syntactic structure."));




    public SyntacticStructure? ConsumeLastStructure()
    {
        var structure = _structures.LastOrDefault();

        if (structure is not null)
            _structures.Remove(structure);

        return structure;
    }




    private Word[] AdvanceSentence()
    {
        var words = new List<Word>();

        while (Peek() is var word && !Splitters.Contains(word.String))
        {
            words.Add(word);
            Advance();
        }

        return words.ToArray();
    }




    public Word Advance()
    {
        if (AtEnd())
            return Previous();

        return Words[_current++];
    }


    public Word Peek()
        => !AtEnd() ? Words[_current] : Previous();


    public Word Previous()
        => Words[_current - 1];


    public Word Next()
    {
        if (_current + 1 >= Words.Length)
            return Peek();

        return Words[_current + 1];
    }


    public bool AtEnd()
        => _current >= Words.Length;
}
