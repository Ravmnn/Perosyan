using System;
using System.Collections.Generic;
using System.Linq;

using RadLine;

using Spectre.Console;
using Spectre.Console.Rendering;

using Psyan.Analyzer;
using Psyan.Analyzer.Structures;


using Boolean = Psyan.Analyzer.Structures.Boolean;


namespace Psyan;




public readonly record struct StyleSpan(TokenLocation Location, Style Style);


public class PisanLineEditorHighlighter : IHighlighter, ISyntacticStructureProcessor
{
    private List<StyleSpan> _spans = [];

    private bool _italic;
    private bool _underline;




    public IRenderable BuildHighlightedText(string text)
    {
        _spans = [];

        var highlightedText = text;

        var tokens = new Lexer(text).Tokenize();
        var words = OrthographicAnalyzer.GetWords(tokens).ToArray();
        var structures = new Parser(words).Parse();

        foreach (var structure in structures)
            Process(structure);

        _spans = _spans.DistinctBy(span => span.Location.Start).ToList();
        _spans = _spans.OrderBy(span => span.Location.Start).Reverse().ToList();


        foreach (var span in _spans)
        {
            highlightedText = highlightedText.Insert(span.Location.End, "[/]");
            highlightedText = highlightedText.Insert(span.Location.Start, $"[{span.Style.ToMarkup()}]");
        }

        return new Markup(highlightedText);
    }




    public void Process(SyntacticStructure structure)
        => structure.Process(this);


    public void ProcessProbability(Probability probability)
    {
        Process(probability.IfParticle);
        Process(probability.SoParticle);
        Process(probability.Splitter);
        Process(probability.Sentence);
    }


    public void ProcessConditional(Conditional conditional)
    {
        Process(conditional.IfParticle);
        Process(conditional.ConjunctionSentence);
    }




    public void ProcessConditionalParticle(ConditionalParticle particle)
    {
        AddWord(particle.Word, ColorPalette.ConditionalParticle);
    }




    public void ProcessConjunction(Conjunction conjunction)
    {
        Process(conjunction.Left);
        Process(conjunction.ConjunctionSplitter);

        Process(conjunction.ConjunctionWord);
        Process(conjunction.ConjunctionLinkSplitter);

        Process(conjunction.Right);
    }




    public void ProcessConjunctionParticle(ConjunctionParticle particle)
    {
        AddWord(particle.Word, ColorPalette.ConjunctionParticle);
    }




    public void ProcessVerb(Verb verb)
    {
        _italic = true;

        verb.Subject?.Process(this);
        verb.MoodParticle.Process(this);
        verb.VerbNoun?.Process(this);
        verb.TenseParticle?.Process(this);
        verb.AspectParticle?.Process(this);
        verb.DestinationParticle?.Process(this);
        verb.Destination?.Process(this);
        verb.ObjectParticle?.Process(this);
        verb.Object?.Process(this);

        _italic = false;
    }




    public void ProcessVerbParticle(VerbParticle particle)
    {
        var color = GetVerbParticleColor(particle);

        AddWord(particle.Word, color);
    }


    private static Style GetVerbParticleColor(VerbParticle particle)
    {
        return particle.ParticleType switch
        {
            VerbParticle.Type.MoodSpecifier => ColorPalette.VerbMoodParticle,
            VerbParticle.Type.TenseSpecifier => ColorPalette.VerbTenseParticle,
            VerbParticle.Type.AspectSpecifier => ColorPalette.VerbAspectParticle,
            VerbParticle.Type.DestinationSpecifier => ColorPalette.VerbDestinationParticle,
            VerbParticle.Type.ObjectSpecifier => ColorPalette.VerbObjectParticle,

            _ => throw new InvalidOperationException("Invalid particle type at rendering")
        };
    }




    public void ProcessPreposition(Preposition preposition)
    {
        Process(preposition.Left);
        Process(preposition.PrepositionParticle);
        Process(preposition.Right);
    }




    public void ProcessPrepositionParticle(PrepositionParticle particle)
    {
        AddWord(particle.Word, ColorPalette.PrepositionParticle);
    }




    public void ProcessPronoun(Pronoun pronoun)
    {
        AddWord(pronoun.Word, ColorPalette.Pronoun);
    }




    public void ProcessNoun(Noun noun)
    {
        AddWord(noun.Word, noun.IsVerb ? ColorPalette.VerbNoun : ColorPalette.Noun);

        if (noun.Adjective is not null)
            AddWord(noun.Adjective, ColorPalette.Adjective);
    }




    public void ProcessBoolean(Boolean boolean)
    {
        AddWord(boolean.Word, ColorPalette.Boolean);
    }




    public void ProcessPunctuationParticle(PunctuationParticle particle)
    {
        AddWord(particle.Word, ColorPalette.PunctuationParticle);
    }




    public void ProcessExpect(Expect expect)
    {
        if (expect.Got is null)
            return;

        var orthographyError = expect.Got.Value.HasError;

        _underline = true;

        var color = orthographyError ? ColorPalette.InvalidOrthography : ColorPalette.Invalid;
        AddWord(expect.Got, color);

        _underline = false;
    }




    private void AddWord(Word? word, Style style)
    {
        style = ApplyDecorations(style);

        if (word is not null)
            _spans.Add(new StyleSpan(word.Value.Token.Location, style));
    }


    private Style ApplyDecorations(Style style)
    {
        if (_italic)
            style = style.Decoration(Decoration.Italic);

        if (_underline)
            style = style.Decoration(Decoration.Underline);

        return style;
    }
}
