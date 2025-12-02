using System;


namespace Psyan.Analyzer.Structures;




public class PunctuationParticle(Word word) : Particle(word)
{
    public Type ParticleType { get; } = TypeOf(word) ?? throw new ArgumentException("Invalid punctuation particle");




    public enum Type
    {
        SentenceSplitter,
        Comma
    }


    public static Type? TypeOf(Word word) => word.String switch
    {
        ";" => Type.SentenceSplitter,
        "," => Type.Comma,

        _ => null
    };


    public static bool IsPunctuationParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessPunctuationParticle(this);
}
