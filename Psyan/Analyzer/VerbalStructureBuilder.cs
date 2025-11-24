namespace Psyan.Analyzer;




public class VerbalStructureBuilder : ISyntacticStructureBuilder
{
    public SyntacticStructure Build(SyntacticAnalyzer analyzer)
    {
        var subject = analyzer.ConsumeLastStructure();

        var moodWord = analyzer.Advance();
        var verbWord = analyzer.Advance();

        var tense = analyzer.Peek().String.StringToVerbTense();
        Word? tenseWord = tense == VerbTense.Invalid ? null : analyzer.Advance();

        // TODO: AtEnd()? then error
        Word? continuous = null;

        if (!analyzer.AtEnd() && analyzer.Peek().String == "na")
            continuous = analyzer.Advance();


        // TODO: objects
        // Word? objectSpecifier = null;
        //
        // if (!analyzer.AtEnd() && analyzer.Peek().String == "li")
        //     continuous = analyzer.Advance();



        return new VerbalStructure(subject, null, verbWord, moodWord, tenseWord, continuous);
    }




    public bool Match(SyntacticAnalyzer analyzer)
        => analyzer.Peek().String.StringToVerbMood() != VerbMood.Invalid;
}
