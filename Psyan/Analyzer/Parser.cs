using System;
using System.Collections.Generic;

using Psyan.Analyzer.Structures;


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
        {
            var structure = ParseConjunction();
            structures.Add(structure);

            if (structure is Expect)
                Advance();
        }

        return structures;
    }




    private SyntacticStructure ParseConjunction()
    {
        var left = ParseVerb();

        if (!Match(Conjunction.IsConjunctionSplitter, out var conjunctionSplitter))
            return left;

        if (!Match(Conjunction.IsConjunction, out var conjunction))
            return left;

        if (!Match(Conjunction.IsConjunctionLinkSplitter, out var conjunctionLinkSplitter))
            return left;

        var right = ParseVerb();

        return new Conjunction(left, right, conjunction, conjunctionSplitter, conjunctionLinkSplitter);
    }




    private SyntacticStructure ParseVerb()
    {
        var subject = ParseNounOrPronoun();

        if (!Match(Verb.IsMood, out var moodWord))
            return subject;

        var verbNoun = ParseNoun();

        MatchOrNull(Verb.IsTense, out var tenseWord);

        var (destinationSpecifier, destination) = ParseVerbArgument("ke");
        var (objectSpecifier, @object) = ParseVerbArgument("li");

        return new Verb(subject, moodWord, verbNoun, tenseWord, destinationSpecifier, destination, objectSpecifier, @object);
    }


    private (Word?, SyntacticStructure?) ParseVerbArgument(string specifier)
    {
        if (!Match(word => word.Token.Lexeme == specifier, out var specifierWord))
            return (null, null);

        var @object = ParseNounOrPronoun();

        return (specifierWord, @object);
    }




    private SyntacticStructure ParseNounOrPronoun(bool advanceIfInvalid = true)
    {
        if (ParseNoun(false) is Noun noun)
            return noun;

        if (ParsePronoun(false) is Pronoun pronoun)
            return pronoun;

        return CreateExpectAndAdvance("Expect noun or pronoun", advanceIfInvalid);
    }




    private SyntacticStructure ParsePronoun(bool advanceIfInvalid = true)
    {
        if (!Match(Pronoun.IsPronoun, out var pronoun))
            return CreateExpectAndAdvance("Expect pronoun", advanceIfInvalid);

        return new Pronoun(pronoun);
    }




    private SyntacticStructure ParseNoun(bool advanceIfInvalid = true)
    {
        var expect = CreateExpect("Expect noun");

        var noun = ParseSingleNoun(advanceIfInvalid);
        var isInvalid = noun is null || noun.Word.HasError;

        if (isInvalid)
            return expect;

        var adjective = ParseSingleNoun(false);

        return new Noun(noun!.Word, adjective?.Word);
    }


    private Noun? ParseSingleNoun(bool advanceIfInvalid = true)
    {
        var word = ParseSingleWord(Noun.IsNoun, advanceIfInvalid);

        if (word is not null)
            return new Noun(word.Value);

        return null;
    }


    private Word? ParseSingleWord(Func<Word, bool> predicate, bool advanceIfInvalid = true)
    {
        if (!Match(predicate, out var word))
        {
            if (advanceIfInvalid)
                Advance();

            return null;
        }

        return word;
    }




    private Expect CreateExpect(string message)
        => new Expect(Previous(), Peek(), message, AtEnd());


    private Expect CreateExpectAndAdvance(string message, bool advance = true)
    {
        var expect = CreateExpect(message);

        if (advance)
            Advance();

        return expect;
    }




    private bool Match(Func<Word, bool> predicate, out Word word)
    {
        if (!AtEnd() && predicate(Peek()!.Value))
        {
            word = Advance()!.Value;
            return true;
        }

        word = default;
        return false;
    }


    private bool MatchOrNull(Func<Word, bool> predicate, out Word? word)
    {
        if (!AtEnd() && predicate(Peek()!.Value))
        {
            word = Advance();
            return true;
        }

        word = null;
        return false;
    }





    private Word? Advance()
    {
        if (AtEnd())
            return null;

        return Words[_current++];
    }


    private Word? Peek()
        => !AtEnd() ? Words[_current] : null;


    private Word? Previous(int amount = 1)
    {
        if (_current <= amount - 1)
            return null;

        return Words[_current - amount];
    }


    private bool AtEnd()
        => _current >= Words.Length;
}
