using System;

namespace Psyan.Analyzer.Structures;




public class ConditionalParticle(Word word) : Particle(word), IParticleValidator, IParticleFactory<ConditionalParticle>
{
    public Type ParticleType { get; } = (Type?)TypeOf(word) ?? throw new ArgumentException("Invalid conditional particle");




    public enum Type
    {
        ConditionParticle,
        CauseParticle
    }


    public static int? TypeOf(Word word) => word.String switch
    {
        "fi" => (int)Type.ConditionParticle,
        "so" => (int)Type.CauseParticle,

        _ => null
    };


    public static bool IsConditionalParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessConditionalParticle(this);




    public static ConditionalParticle Create(Word word)
        => new ConditionalParticle(word);
}
