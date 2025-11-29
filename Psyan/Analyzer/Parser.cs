namespace Psyan.Analyzer;




public class Parser(Word[] words)
{
    private int _current;


    public Word[] Words { get; } = words;




    public IEnumerable<SyntacticStructure> Parse()
    {
        _current = 0;

        var structures = new List<SyntacticStructure>();

        while (!AtEnd())
            structures.Add(ParseVerb());

        return structures;
    }




    private SyntacticStructure ParseVerb()
    {
        var subject = ParseNounOrPronoun();

        if (!Match(Verb.IsMood, out var moodWord))
            return subject;

        var nounStructure = ParseNoun();

        if (nounStructure is not Noun verbNounWord)
            return nounStructure;

        MatchOrNull(Verb.IsTense, out var tenseWord);

        var (destinationSpecifier, destination) = ParseVerbArgument("ke");
        var (objectSpecifier, @object) = ParseVerbArgument("li");

        return new Verb(subject, moodWord, verbNounWord.Word, tenseWord, destinationSpecifier, destination, objectSpecifier, @object);
    }


    private (Word?, SyntacticStructure?) ParseVerbArgument(string specifier)
    {
        if (!Match(word => word.Token.Lexeme == specifier, out var specifierWord))
            return (null, null);

        var @object = ParseNounOrPronoun();

        return (specifierWord, @object);
    }




    private SyntacticStructure ParseNounOrPronoun()
    {
        if (ParseNoun() is Noun noun)
            return noun;

        if (ParsePronoun() is Pronoun pronoun)
            return pronoun;

        return AdvanceInvalid("Expect noun or pronoun");
    }




    private SyntacticStructure ParsePronoun()
    {
        if (!Match(Pronoun.IsPronoun, out var pronoun))
            return CreateInvalid("Invalid pronoun");

        return new Pronoun(pronoun);
    }




    private SyntacticStructure ParseNoun()
    {
        var noun = ParseSingleNoun();
        var adjective = ParseSingleNoun();

        if (noun is null)
            return CreateInvalid("Invalid noun");

        return new Noun(noun.Word, adjective?.Word);
    }


    private Noun? ParseSingleNoun()
    {
        if (!Match(Noun.IsNoun, out var noun))
            return null;

        return new Noun(noun);
    }




    private Invalid CreateInvalid(string message)
        => new Invalid(Peek(), message);


    private Invalid AdvanceInvalid(string message)
    {
        Advance();
        return CreateInvalid(message);
    }


    private bool Match(Func<Word, bool> predicate, out Word word)
    {
        if (predicate(Peek()))
        {
            word = Advance();
            return true;
        }

        word = default;
        return false;
    }


    private bool MatchOrNull(Func<Word, bool> predicate, out Word? word)
    {
        if (predicate(Peek()))
        {
            word = Advance();
            return true;
        }

        word = null;
        return false;
    }





    private bool Check(TokenType token)
    {
        if (AtEnd())
            return false;

        return Peek().Token.Type == token;
    }


    private Word Advance()
    {
        if (AtEnd())
            return Previous();

        return Words[_current++];
    }


    private Word Peek()
        => !AtEnd() ? Words[_current] : Previous();


    private Word Previous(int amount = 1)
    {
        if (_current <= amount - 1)
            return Peek();

        return Words[_current - amount];
    }


    private bool AtEnd()
        => _current >= Words.Length;
}
