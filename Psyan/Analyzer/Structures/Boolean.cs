using System;

namespace Psyan.Analyzer.Structures;




public class Boolean(Word word) : SyntacticStructure
{
    public Word Word { get; } = word;

    public bool Value { get; } = BooleanValueOf(word) ?? throw new ArgumentException("Invalid boolean word");




    public static bool? BooleanValueOf(Word word) => word.String switch
    {
        "si" => true,
        "no" => false,

        _ => null
    };


    public static bool IsBoolean(Word word)
        => BooleanValueOf(word) is not null;




    public override void Process(ISyntacticStructureProcessor processor)
        => processor.ProcessBoolean(this);


    public override Word BaseWord()
        => Word;
}
