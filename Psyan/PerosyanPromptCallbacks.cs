using PrettyPrompt;
using PrettyPrompt.Highlighting;

using Psyan.Analyzer;


namespace Psyan;




public class PerosyanPromptCallbacks : PromptCallbacks, ISyntacticStructureProcessor
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


    private static bool ProcessedPunctuation(List<FormatSpan> spans, Token token)
    {
        if (token.Type == TokenType.Punctuation)
        {
            spans.Add(new FormatSpan(token.Location.Start, 1, AnsiColor.BrightBlack));
            return true;
        }

        return false;
    }


    private static bool ProcessedNumber(List<FormatSpan> spans, Token token)
    {
        if (token.Type == TokenType.Number)
        {
            spans.Add(new FormatSpan(token.Location.Start, token.Lexeme.Length, AnsiColor.Yellow));
            return true;
        }

        return false;
    }


    private static void AddErrorFormatting(List<FormatSpan> spans, Token token, int errorIndex)
    {
        var absoluteIndex = errorIndex + token.Location.Start;

        var atLastCharacter = errorIndex == token.Lexeme.Length - 1;

        spans.Add(new FormatSpan(token.Location.Start, errorIndex, AnsiColor.Red));

        if (!atLastCharacter)
            spans.Add(new FormatSpan(absoluteIndex + 1, token.Lexeme.Length - errorIndex - 1, AnsiColor.Red));

        spans.Add(new FormatSpan(absoluteIndex, 1, new ConsoleFormat(AnsiColor.Red, Underline: true)));
    }




    public void Process(SyntacticStructure structure)
        => structure.Process(this);


    public void ProcessVerb(Verb verb)
    {
        verb.Subject?.Process(this);

        AddSpan(verb.MoodWord.Token.Location, AnsiColor.Green);
        AddSpan(verb.VerbNounWord.Token.Location, AnsiColor.Blue);

        if (verb.TenseWord is not null)
            AddSpan(verb.TenseWord.Value.Token.Location, AnsiColor.Magenta);

        verb.Destination?.Process(this);
        verb.Object?.Process(this);
    }


    public void ProcessPronoun(Pronoun pronoun)
    {
        AddSpan(pronoun.Word.Token.Location, AnsiColor.Yellow);
    }


    public void ProcessNoun(Noun noun)
    {
        AddSpan(noun.Word.Token.Location, AnsiColor.Blue);

        if (noun.Adjective is not null)
            AddSpan(noun.Adjective.Value.Token.Location, AnsiColor.BrightCyan);
    }


    public void ProcessInvalid(Invalid invalid)
    {
        AddSpan(invalid.Word.Token.Location, AnsiColor.Red);
    }




    private void AddSpan(TokenLocation location, AnsiColor color)
        => _spans.Add(new FormatSpan(location.Start, location.Length, color));
}
