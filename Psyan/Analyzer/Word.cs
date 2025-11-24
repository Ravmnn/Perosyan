namespace Psyan.Analyzer;




public readonly struct Word(Token token, Syllable[] syllables)
{
    public Token Token { get; } = token;
    public string String => Token.Lexeme;

    public Syllable[] Syllables { get; } = syllables;


    public InvalidSyllable? Error
        => Syllables.FirstOrDefault(syllable => syllable is InvalidSyllable) as InvalidSyllable;

    public bool HasError => Error is not null;




    public override string ToString()
        => $"{String}: {string.Join('-', Syllables.Cast<object?>())}";
}
