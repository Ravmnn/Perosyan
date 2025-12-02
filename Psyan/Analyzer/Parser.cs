using System;
using System.Collections.Generic;

using Psyan.Analyzer.Structures;


using Boolean = Psyan.Analyzer.Structures.Boolean;


namespace Psyan.Analyzer;

// TODO: "fi so, ..." for probability
// TODO: add prepositions




public class Parser(Word[] words)
{
    private int _current;


    public Word[] Words { get; } = words;




    public IEnumerable<SyntacticStructure> Parse()
    {
        _current = 0;

        var structures = new List<SyntacticStructure>();

        while (!AtEnd())
            structures.Add(ParseConditional());

        return structures;
    }




    private SyntacticStructure ParseConditional()
    {
        var ifParticle = ParseConditionalParticle(ConditionalParticle.Type.ConditionParticle);

        if (ifParticle is Expect)
            return ParseConjunction();

        var conjunctionSentence = ParseConjunction(ConjunctionParticle.Type.CausalForward);

        return new Conditional(ifParticle, conjunctionSentence);
    }


    private SyntacticStructure ParseConditionalParticle(ConditionalParticle.Type particleType)
    {
        if (!Match(ConditionalParticle.IsConditionalParticle, out var word, false) || ConditionalParticle.TypeOf(word) != particleType)
            return CreateExpect($"Expect conditional particle ({particleType})");

        Advance();
        return new ConditionalParticle(word);
    }




    private SyntacticStructure ParseConjunction(ConjunctionParticle.Type? conjunctionType = null)
    {
        var left = ParseVerb();

        var sentenceSplitter = ParsePunctuationParticle(PunctuationParticle.Type.SentenceSplitter);
        var conjunction = ParseConjunctionParticle(conjunctionType);
        var conjunctionSplitter = ParsePunctuationParticle(PunctuationParticle.Type.Comma);

        // TODO: improve error logging. it's not working very well here

        if (sentenceSplitter is Expect || conjunction is Expect || conjunctionSplitter is Expect)
            return left;

        var right = ParseVerb();

        return new Conjunction(left, right, conjunction, sentenceSplitter, conjunctionSplitter);
    }


    private SyntacticStructure ParseConjunctionParticle(ConjunctionParticle.Type? conjunctionType = null)
    {
        if (!Match(ConjunctionParticle.IsConjunctionParticle, out var word, false))
            return CreateExpect("Expect conjunction particle");

        if (conjunctionType is not null && ConjunctionParticle.TypeOf(word) != conjunctionType)
            return CreateExpect($"Expect conjunction particle ({conjunctionType})");

        Advance();
        return new ConjunctionParticle(word);
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
            return CreateExpect($"Expect verb particle ({particleType})");

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




    // TODO: maybe it's possible to create a generic method for particle parsing
    private SyntacticStructure ParsePunctuationParticle(PunctuationParticle.Type? punctuationType = null)
    {
        if (!Match(PunctuationParticle.IsPunctuationParticle, out var word, false))
            return CreateExpect("Expect punctuation particle");

        if (punctuationType is not null && PunctuationParticle.TypeOf(word) != punctuationType)
            return CreateExpect($"Expect punctuation particle ({punctuationType})");

        Advance();
        return new PunctuationParticle(word);
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
