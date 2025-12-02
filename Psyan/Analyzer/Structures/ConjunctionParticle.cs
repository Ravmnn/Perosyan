using System;

namespace Psyan.Analyzer.Structures;




public class ConjunctionParticle(Word word) : Particle(word)
{
    public Type ParticleType { get; } = TypeOf(word) ?? throw new ArgumentException("Invalid conjunction particle");




    public enum Type
    {
        CausalForward,
        CausalBackward,
        Adversative,
        Sequential
    }


    public static Type? TypeOf(Word word) => word.String switch
    {
        "so" => Type.CausalForward,
        "ko" => Type.CausalBackward,
        "ma" => Type.Adversative,
        "ta" => Type.Sequential,

        _ => null
    };


    public static bool IsConjunctionParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConjunctionParticle(this);
}
