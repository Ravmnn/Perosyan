using System;

namespace Psyan.Analyzer.Structures;




public class ConditionalParticle(Word word) : Particle(word)
{
    public Type ParticleType { get; } = TypeOf(word) ?? throw new ArgumentException("Invalid conditional particle");




    public enum Type
    {
        ConditionParticle,
        CauseParticle
    }


    public static Type? TypeOf(Word word) => word.String switch
    {
        "fi" => Type.ConditionParticle,
        "so" => Type.CauseParticle,

        _ => null
    };


    public static bool IsConditionalParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConditionalParticle(this);
}
