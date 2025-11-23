namespace Psyan.Analyzer;




public interface ISyntacticStructureBuilder
{
    SyntacticStructure Build(SyntacticAnalyzer analyzer);

    bool Match(SyntacticAnalyzer analyzer);
}
