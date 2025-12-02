using System;

namespace Psyan.Analyzer.Structures;




public class VerbParticle(Word word) : Particle(word)
{
    public Type ParticleType { get; } = TypeOf(word) ?? throw new ArgumentException("Invalid verb particle");




    public enum Type
    {
        MoodSpecifier,
        TenseSpecifier,
        AspectSpecifier,
        DestinationSpecifier,
        ObjectSpecifier
    }


    public static Type? TypeOf(Word word) => word.String switch
    {
        not null when Verb.IsMood(word) => Type.MoodSpecifier,
        not null when Verb.IsTense(word) => Type.TenseSpecifier,
        not null when Verb.IsAspect(word) => Type.AspectSpecifier,

        "ke" => Type.DestinationSpecifier,
        "li" => Type.ObjectSpecifier,

        _ => null
    };


    public static bool IsVerbParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessVerbParticle(this);
}
