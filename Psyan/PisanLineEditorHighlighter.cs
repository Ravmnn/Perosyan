using System;
using System.Collections.Generic;
using System.Linq;

using RadLine;

using Spectre.Console;
using Spectre.Console.Rendering;

using Psyan.Analyzer;
using Psyan.Analyzer.Structures;


namespace Psyan;




public readonly record struct StyleSpan(TokenLocation Location, Style Style);


public class PisanLineEditorHighlighter : IHighlighter, ISyntacticStructureProcessor
{
    private List<StyleSpan> _spans = [];




    public IRenderable BuildHighlightedText(string text)
    {
        _spans = [];

        var highlightedText = text;

        var tokens = new Lexer(text).Tokenize();
        var words = OrthographicAnalyzer.GetWords(tokens).ToArray();
        var structures = new Parser(words).Parse();

        foreach (var structure in structures)
            Process(structure);

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


    public void ProcessConjunction(Conjunction conjunction)
    {
        Process(conjunction.Left);
        AddWord(conjunction.ConjunctionSplitter, ColorPalette.Punctuaction);

        AddWord(conjunction.ConjunctionWord, ColorPalette.Conjunction);
        AddWord(conjunction.ConjunctionLinkSplitter, ColorPalette.Punctuaction);

        Process(conjunction.Right);
    }




    public void ProcessVerb(Verb verb)
    {
        verb.Subject?.Process(this);
        verb.MoodWord.Process(this);
        verb.VerbNoun?.Process(this);
        verb.TenseWord?.Process(this);
        verb.DestinationSpecifier?.Process(this);
        verb.Destination?.Process(this);
        verb.ObjectSpecifier?.Process(this);
        verb.Object?.Process(this);
    }




    public void ProcessPronoun(Pronoun pronoun)
    {
        AddWord(pronoun.Word, ColorPalette.Pronoun);
    }




    public void ProcessNoun(Noun noun)
    {
        AddWord(noun.Word, ColorPalette.Noun);

        if (noun.Adjective is not null)
            AddWord(noun.Adjective, ColorPalette.Adjective);
    }




    public void ProcessExpect(Expect expect)
    {
        if (expect.Got is null)
            return;

        var color = expect.Got.Value.HasError ? ColorPalette.InvalidOrthography : ColorPalette.Invalid;
        AddWord(expect.Got, color);
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
            VerbParticle.Type.MoodSpecifier => ColorPalette.VerbMood,
            VerbParticle.Type.TenseSpecifier => ColorPalette.VerbTense,
            VerbParticle.Type.AspectSpecifier => Color.Black,
            VerbParticle.Type.DestinationSpecifier => ColorPalette.VerbDestinationSpecifier,
            VerbParticle.Type.ObjectSpecifier => ColorPalette.VerbObjectSpecifier,

            _ => throw new InvalidOperationException("Invalid particle type at rendering")
        };
    }




    private void AddWord(Word? word, Style style)
    {
        if (word is not null)
            _spans.Add(new StyleSpan(word.Value.Token.Location, style));
    }
}
