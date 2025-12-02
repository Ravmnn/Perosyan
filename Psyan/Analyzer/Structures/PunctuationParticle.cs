using System;


namespace Psyan.Analyzer.Structures;




public class PunctuationParticle(Word word) : Particle(word), IParticleValidator, IParticleFactory<PunctuationParticle>
{
    public Type ParticleType { get; } = (Type?)TypeOf(word) ?? throw new ArgumentException("Invalid punctuation particle");




    public enum Type
    {
        SentenceSplitter,
        Comma
    }


    public static int? TypeOf(Word word) => word.String switch
    {
        ";" => (int)Type.SentenceSplitter,
        "," => (int)Type.Comma,

        _ => null
    };




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessPunctuationParticle(this);




    public static PunctuationParticle Create(Word word)
        => new PunctuationParticle(word);
}
