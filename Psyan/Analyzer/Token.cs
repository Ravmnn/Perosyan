namespace Psyan.Analyzer;




public enum TokenType
{
    Invalid,
    Word,
    Number,
    Punctuation
}


public readonly record struct TokenLocation(int Start, int End)
{
    public int Length => End - Start;
}

public readonly record struct Token(string Lexeme, TokenLocation Location, TokenType Type);
