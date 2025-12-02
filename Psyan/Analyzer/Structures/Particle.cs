namespace Psyan.Analyzer.Structures;




public interface IParticleValidator
{
    static abstract int? TypeOf(Word word);
}


public interface IParticleFactory<out T>
{
    static abstract T Create(Word word);
}




public abstract class Particle(Word word) : SyntacticStructure
{
    public Word Word { get; } = word;




    public override Word BaseWord()
        => Word;
}
