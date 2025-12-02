using System;
using System.Collections.Generic;

using Psyan.Analyzer.Structures;


using Boolean = Psyan.Analyzer.Structures.Boolean;


namespace Psyan.Analyzer;

// TODO: add boolean particles
// TODO: add conditional sentences "fi? ...(; so, ...)?", and "fi so, ..." for probability
// TODO: add A du B




public class Parser(Word[] words)
{
    private int _current;


    public Word[] Words { get; } = words;




    public IEnumerable<SyntacticStructure> Parse()
    {
        _current = 0;

        var structures = new List<SyntacticStructure>();

        while (!AtEnd())
            structures.Add(ParseConjunction());

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
        var subject = ParsePrimitive();
        var moodParticle = ParseVerbParticle(VerbParticle.Type.MoodSpecifier);

        if (moodParticle is Expect)
            return subject;

        var verbNoun = ParseNoun(true);

        var tenseParticle = ParseVerbParticleOrNull(VerbParticle.Type.TenseSpecifier);
        var aspectParticle = ParseVerbParticleOrNull(VerbParticle.Type.AspectSpecifier);

        var (destinationParticle, destination) = ParseVerbArgument(VerbParticle.Type.DestinationSpecifier);
        var (objectParticle, @object) = ParseVerbArgument(VerbParticle.Type.ObjectSpecifier);

        return new Verb(subject, moodParticle, verbNoun, tenseParticle, aspectParticle, destinationParticle, destination, objectParticle, @object);
    }


    private SyntacticStructure ParseVerbParticle(VerbParticle.Type particleType)
    {
        if (!Match(VerbParticle.IsVerbParticle, out var word, false) || VerbParticle.TypeOf(word) != particleType)
            return CreateExpectAndAdvance($"Expect verb particle ({particleType})", false);

        Advance();
        return new VerbParticle(word);
    }


    private SyntacticStructure? ParseVerbParticleOrNull(VerbParticle.Type particleType)
        => ParseVerbParticle(particleType) as VerbParticle;


    private (SyntacticStructure?, SyntacticStructure?) ParseVerbArgument(VerbParticle.Type particleType)
    {
        var particle = ParseVerbParticleOrNull(particleType);
        SyntacticStructure? @object = null;

        if (particle is not null)
            @object = ParsePrimitive();

        return (particle, @object);
    }




    private SyntacticStructure ParsePrimitive(bool advanceIfInvalid = true)
    {
        if (ParseBoolean(false) is Boolean boolean)
            return boolean;

        if (ParseNoun(advanceIfInvalid: false) is Noun noun)
            return noun;

        if (ParsePronoun(false) is Pronoun pronoun)
            return pronoun;

        return CreateExpectAndAdvance("Expect primitive (nouns, pronouns, booleans...)", advanceIfInvalid);
    }




    private SyntacticStructure ParsePronoun(bool advanceIfInvalid = true)
    {
        if (!Match(Pronoun.IsPronoun, out var pronoun))
            return CreateExpectAndAdvance("Expect pronoun", advanceIfInvalid);

        return new Pronoun(pronoun);
    }




    private SyntacticStructure ParseNoun(bool isVerb = false, bool advanceIfInvalid = true)
    {
        var expect = CreateExpect("Expect noun");

        var noun = ParseSingleNoun(advanceIfInvalid);
        var isInvalid = noun is null || noun.Word.HasError;

        if (isInvalid)
            return expect;

        var adjective = ParseSingleNoun(false);

        return new Noun(noun!.Word, adjective?.Word, isVerb);
    }


    private Noun? ParseSingleNoun(bool advanceIfInvalid = true)
    {
        var word = ParseSingleWord(Noun.IsNoun, advanceIfInvalid);

        if (word is not null)
            return new Noun(word.Value);

        return null;
    }




    private SyntacticStructure ParseBoolean(bool advanceIfInvalid = true)
    {
        if (ParseSingleWord(Boolean.IsBoolean, advanceIfInvalid) is not { } boolean)
            return CreateExpectAndAdvance("Expect boolean", advanceIfInvalid);

        return new Boolean(boolean);
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




    private bool Match(Func<Word, bool> predicate, out Word word, bool advance = true)
    {
        if (!AtEnd() && predicate(Peek()!.Value))
        {
            word = Peek()!.Value;

            if (advance)
                Advance();

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
