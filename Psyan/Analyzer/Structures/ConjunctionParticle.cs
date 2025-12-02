using System;

namespace Psyan.Analyzer.Structures;




public class ConjunctionParticle(Word word) : Particle(word), IParticleValidator, IParticleFactory<ConjunctionParticle>
{
    public Type ParticleType { get; } = (Type?)TypeOf(word) ?? throw new ArgumentException("Invalid conjunction particle");




    public enum Type
    {
        CausalForward,
        CausalBackward,
        Adversative,
        Sequential
    }


    public static int? TypeOf(Word word) => word.String switch
    {
        "so" => (int)Type.CausalForward,
        "ko" => (int)Type.CausalBackward,
        "ma" => (int)Type.Adversative,
        "ta" => (int)Type.Sequential,

        _ => null
    };




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConjunctionParticle(this);




    public static ConjunctionParticle Create(Word word)
        => new ConjunctionParticle(word);
}
