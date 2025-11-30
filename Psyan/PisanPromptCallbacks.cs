using PrettyPrompt;
using PrettyPrompt.Highlighting;

using Psyan.Analyzer;


namespace Psyan;




public class PisanPromptCallbacks : PromptCallbacks, ISyntacticStructureProcessor
{
    private List<FormatSpan> _spans = [];

    // TODO: add number and punctuation support.




    protected override Task<IReadOnlyCollection<FormatSpan>> HighlightCallbackAsync(string text, CancellationToken _)
    {
        _spans = [];

        var tokens = new Lexer(text).Tokenize();
        var words = OrthographicAnalyzer.GetWords(tokens);
        var structures = new Parser(words.ToArray()).Parse();

        foreach (var structure in structures)
            Process(structure);

        return Task.FromResult<IReadOnlyCollection<FormatSpan>>(_spans.AsReadOnly());
    }




    public void Process(SyntacticStructure structure)
        => structure.Process(this);


    public void ProcessConjunction(Conjunction conjunction)
    {
        Process(conjunction.Left);
        Process(conjunction.Right);

        AddSpan(conjunction.ConjunctionSplitter.Token.Location, ColorPalette.Punctuaction);
        AddSpan(conjunction.ConjunctionLinkSplitter.Token.Location, ColorPalette.Punctuaction);

        AddSpan(conjunction.ConjunctionWord.Token.Location, ColorPalette.Conjunction);
    }


    public void ProcessVerb(Verb verb)
    {
        verb.Subject?.Process(this);

        AddSpan(verb.MoodWord.Token.Location, ColorPalette.VerbMood);
        AddSpan(verb.VerbNounWord.Token.Location, ColorPalette.VerbNoun);

        if (verb.TenseWord is not null)
            AddSpan(verb.TenseWord.Value.Token.Location, ColorPalette.VerbTense);

        if (verb.DestinationSpecifier is not null)
            AddSpan(verb.DestinationSpecifier.Value.Token.Location, ColorPalette.VerbDestinationSpecifier);

        if (verb.ObjectSpecifier is not null)
            AddSpan(verb.ObjectSpecifier.Value.Token.Location, ColorPalette.VerbObjectSpecifier);

        verb.Destination?.Process(this);
        verb.Object?.Process(this);
    }


    public void ProcessPronoun(Pronoun pronoun)
    {
        AddSpan(pronoun.Word.Token.Location, ColorPalette.Pronoun);
    }


    public void ProcessNoun(Noun noun)
    {
        AddSpan(noun.Word.Token.Location, ColorPalette.Noun);

        if (noun.Adjective is not null)
            AddSpan(noun.Adjective.Value.Token.Location, ColorPalette.Adjective);
    }


    public void ProcessInvalid(Invalid invalid)
    {
        var color = invalid.Word.HasError ? ColorPalette.InvalidOrthography : ColorPalette.Invalid;
        AddSpan(invalid.Word.Token.Location, color);
    }




    private void AddSpan(TokenLocation location, AnsiColor color)
        => _spans.Add(new FormatSpan(location.Start, location.Length, color));
}
