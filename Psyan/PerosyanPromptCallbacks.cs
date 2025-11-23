using PrettyPrompt;
using PrettyPrompt.Highlighting;

using Psyan.Analyzer;


namespace Psyan;




public class PerosyanPromptCallbacks : PromptCallbacks
{
    protected override Task<IReadOnlyCollection<FormatSpan>> HighlightCallbackAsync(string text, CancellationToken cancellationToken)
    {
        var spans = new List<FormatSpan>();

        var tokens = new Lexer(text).Tokenize();
        var orthography = new OrthographicAnalyzer();

        foreach (var token in tokens)
        {
            if (ProcessedPunctuation(spans, token) || ProcessedNumber(spans, token))
                continue;

            orthography.Word = token.Lexeme;

            if (orthography.GetOrthographyErrorIndex() is { } errorIndex)
                AddErrorFormatting(spans, token, errorIndex);
        }

        return Task.FromResult<IReadOnlyCollection<FormatSpan>>(spans.AsReadOnly());
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
}
