using System;

namespace Psyan.Analyzer.Structures;




public class VerbParticle(Word word) : Particle(word), IParticleValidator, IParticleFactory<VerbParticle>
{
    public Type ParticleType { get; } = (Type?)TypeOf(word) ?? throw new ArgumentException("Invalid verb particle");




    public enum Type
    {
        MoodSpecifier,
        TenseSpecifier,
        AspectSpecifier,
        DestinationSpecifier,
        ObjectSpecifier
    }


    public static int? TypeOf(Word word) => word.String switch
    {
        not null when Verb.IsMood(word) => (int)Type.MoodSpecifier,
        not null when Verb.IsTense(word) => (int)Type.TenseSpecifier,
        not null when Verb.IsAspect(word) => (int)Type.AspectSpecifier,

        "ke" => (int)Type.DestinationSpecifier,
        "li" => (int)Type.ObjectSpecifier,

        _ => null
    };


    public static bool IsVerbParticle(Word word)
        => TypeOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessVerbParticle(this);




    public static VerbParticle Create(Word word)
        => new VerbParticle(word);
}
