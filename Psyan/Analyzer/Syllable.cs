namespace Psyan.Analyzer;




public class Syllable(string word, int start, int end)
{
    public string Word { get; } = word;
    public string SubString => Word[Start .. End];

    public int Start { get; } = start;
    public int End { get; } = end;
}




public class InvalidSyllable(string word, int start, int end, string errorMessage)
    : Syllable(word, start, end)
{
    public string ErrorMessage { get; } = errorMessage;
}
