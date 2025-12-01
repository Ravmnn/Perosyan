using System;
using System.Collections.Generic;


namespace Psyan.Analyzer;




public class Lexer(string source)
{
    private int _start, _end;


    public string Source { get; set; } = source;




    private void Reset()
    {
        _start = _end = 0;
    }




    public IEnumerable<Token> Tokenize()
    {
        var tokens = new List<Token>();
        Reset();

        while (!AtEnd())
        {
            if (TokenizeNext() is { } token)
                tokens.Add(token);

            _start = _end;
        }

        return tokens;
    }


    private Token? TokenizeNext()
    {
        if (Advance() is not { } @char || char.IsWhiteSpace(@char))
            return null;

        if (char.IsAsciiLetter(@char))
            return TokenizeIdentifier(char.IsAsciiDigit, TokenType.Word);

        if (char.IsAsciiDigit(@char))
            return TokenizeIdentifier(char.IsAsciiLetter, TokenType.Number);

        return TokenFromType(TokenType.Punctuation);
    }



    private Token? TokenizeIdentifier(Func<char, bool> invalidator, TokenType type)
    {
        char ch;
        var isInvalid = false;

        while (!AtEnd() && char.IsAsciiLetterOrDigit(ch = Peek()!.Value))
        {
            if (invalidator(ch))
                isInvalid = true;

            Advance();
        }

        return TokenFromType(isInvalid ? TokenType.Invalid : type);
    }




    private Token TokenFromType(TokenType type)
        => new Token(CurrentLexeme(), CurrentLocation(), type);


    private string CurrentLexeme()
        => Source[_start .. _end];


    private TokenLocation CurrentLocation()
        => new TokenLocation(_start, _end);




    private char? Advance()
    {
        if (AtEnd())
            return null;

        return Source[_end++];
    }


    private char? Peek()
        => AtEnd() ? null : Source[_end];


    private bool AtEnd()
        => _end >= Source.Length;
}
