namespace Psyan.Analyzer.Structures;




public class PrepositionParticle(Word word) : Particle(word), IParticleValidator, IParticleFactory<PrepositionParticle>
{
    public enum Type
    {
        PartOf,
        InsideOf,
        With,
        MadeBy,
        For,
        When
    }


    public static int? TypeOf(Word word) => word.String switch
    {
        "du" => (int)Type.PartOf,
        "ne" => (int)Type.InsideOf,
        "gu" => (int)Type.With,
        "bi" => (int)Type.MadeBy,
        "to" => (int)Type.For,
        "be" => (int)Type.When,

        _ => null
    };




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessPrepositionParticle(this);




    public static PrepositionParticle Create(Word word)
        => new PrepositionParticle(word);
}
