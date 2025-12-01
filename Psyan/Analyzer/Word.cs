using System.Linq;


namespace Psyan.Analyzer;




public readonly record struct Word(Token Token, Syllable[] Syllables)
{
    public string String => Token.Lexeme;



    public InvalidSyllable? Error
        => Syllables.FirstOrDefault(syllable => syllable is InvalidSyllable) as InvalidSyllable;

    public bool HasError => Error is not null;
    public bool IsPunctuation => String is "," or "." or ";";




    public override string ToString()
        => $"{String}: {string.Join('-', Syllables.Cast<object?>())}";
}
