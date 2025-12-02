namespace Psyan.Analyzer.Structures;




public abstract class Particle(Word word) : SyntacticStructure
{
    public Word Word { get; } = word;




    public static bool IsParticle(Word word)
        => word.Syllables.Length <= 1 && !word.HasError;


    public override Word BaseWord()
        => Word;
}
