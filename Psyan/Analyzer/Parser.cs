using System;
using System.Collections.Generic;

using Psyan.Analyzer.Structures;


using Boolean = Psyan.Analyzer.Structures.Boolean;


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
            structures.Add(ParseProbability());

        return structures;
    }




    private SyntacticStructure ParseProbability()
    {
        if (ParseConditionalParticle(ConditionalParticle.Type.ConditionParticle) is not ConditionalParticle @if)
            return ParseConjunction();

        if (ParseConditionalParticle(ConditionalParticle.Type.CauseParticle) is not ConditionalParticle so)
            return ParseConditional(@if);

        var splitter = ParsePunctuationParticle(PunctuationParticle.Type.Comma);
        var sentence = ParseConjunction();

        return new Probability(@if, so, splitter, sentence);
    }


    private SyntacticStructure ParseConditional(ConditionalParticle @if)
    {
        var conjunctionSentence = ParseConjunction(ConjunctionParticle.Type.CausalForward);

        return new Conditional(@if, conjunctionSentence);
    }


    private SyntacticStructure ParseConditionalParticle(ConditionalParticle.Type? particleType = null)
        => ParseParticle<ConditionalParticle>("Expect conjunction particle", (int?)particleType);




    private SyntacticStructure ParseConjunction(ConjunctionParticle.Type? conjunctionType = null)
    {
        var left = ParseVerb();

        while (ParsePunctuationParticle(PunctuationParticle.Type.SentenceSplitter) is PunctuationParticle sentenceSplitter)
        {
            var conjunction = ParseConjunctionParticle(conjunctionType);
            var conjunctionSplitter = ParsePunctuationParticle(PunctuationParticle.Type.Comma);

            var right = ParseVerb();

            left = new Conjunction(left, right, conjunction, sentenceSplitter, conjunctionSplitter);
        }

        return left;
    }


    private SyntacticStructure ParseConjunctionParticle(ConjunctionParticle.Type? particleType = null)
        => ParseParticle<ConjunctionParticle>("Expect conjunction particle", (int?)particleType);




    private SyntacticStructure ParseVerb()
    {
        var subject = ParsePreposition();

        if (ParseVerbParticle(VerbParticle.Type.MoodSpecifier) is not VerbParticle moodParticle)
            return subject;

        var verbNoun = ParseNoun(true);

        var tenseParticle = ParseVerbParticleOrNull(VerbParticle.Type.TenseSpecifier);
        var aspectParticle = ParseVerbParticleOrNull(VerbParticle.Type.AspectSpecifier);

        var (destinationParticle, destination) = ParseVerbArgument(VerbParticle.Type.DestinationSpecifier);
        var (objectParticle, @object) = ParseVerbArgument(VerbParticle.Type.ObjectSpecifier);

        return new Verb(subject, moodParticle, verbNoun, tenseParticle, aspectParticle, destinationParticle, destination, objectParticle, @object);
    }


    private SyntacticStructure ParseVerbParticle(VerbParticle.Type? particleType = null)
        => ParseParticle<VerbParticle>("Expect verb particle", (int?)particleType);


    private SyntacticStructure? ParseVerbParticleOrNull(VerbParticle.Type particleType)
        => ParseVerbParticle(particleType) as VerbParticle;


    private (SyntacticStructure?, SyntacticStructure?) ParseVerbArgument(VerbParticle.Type particleType)
    {
        var particle = ParseVerbParticleOrNull(particleType);
        SyntacticStructure? @object = null;

        if (particle is not null)
            @object = ParsePreposition();

        return (particle, @object);
    }




    private SyntacticStructure ParsePreposition()
    {
        var left = ParsePrimitive();

        while (ParsePrepositionParticle() is PrepositionParticle preposition)
        {
            var right = ParsePrimitive();
            left = new Preposition(left, right, preposition);
        }

        return left;
    }


    private SyntacticStructure ParsePrepositionParticle(PrepositionParticle.Type? particleType = null)
        => ParseParticle<PrepositionParticle>("Expect preposition particle", (int?)particleType);




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
        if (!Match(Noun.IsNoun, out var word))
        {
            if (advanceIfInvalid)
                Advance();

            return null;
        }

        return new Noun(word);
    }




    private SyntacticStructure ParseBoolean(bool advanceIfInvalid = true)
    {
        if (!Match(Boolean.IsBoolean, out var word))
            return CreateExpectAndAdvance("Expect boolean", advanceIfInvalid);

        return new Boolean(word);
    }




    private SyntacticStructure ParsePunctuationParticle(PunctuationParticle.Type? punctuationType = null)
        => ParseParticle<PunctuationParticle>("Expect punctuation particle", (int?)punctuationType);




    private SyntacticStructure ParseParticle<T>(string message, int? typeId = null)
        where T : SyntacticStructure, IParticleValidator, IParticleFactory<T>
    {
        if (!Match(word => T.TypeOf(word) is not null, out var particle, false))
            return CreateExpect(message);

        if (typeId is not null && T.TypeOf(particle) != typeId)
            return CreateExpect(message);

        Advance();
        return T.Create(particle);
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
