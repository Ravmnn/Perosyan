namespace Psyan.Analyzer;




public class VerbalStructure(
    SyntacticStructure? subject, SyntacticStructure? @object,
    Word verbWord, Word moodWord, Word? tenseWord, Word? continuousWord = null, Word? objectSpecifierWord = null
)
    : SyntacticStructure([])
{
    // TODO: register error if continuousWord is not "na"

    public override Word[] Words
    {
        get
        {
            var subjectWords = Subject?.Words ?? [];
            var thisWords = new List<Word> { MoodWord, VerbWord };

            if (TenseWord is { } tenseWord)
                thisWords.Add(tenseWord);

            if (ContinuousWord is { } continuousWord)
                thisWords.Add(continuousWord);

            // TODO: add objects here too

            // TODO: please cleanup everything here; maybe re design some things

            return subjectWords.Concat(thisWords).ToArray();
        }
    }


    public SyntacticStructure? Subject { get; } = subject;
    public SyntacticStructure? Object { get; } = @object;
    public Word VerbWord { get; } = verbWord;
    public Word MoodWord { get; } = moodWord;
    public Word? TenseWord { get; } = tenseWord;
    public Word? ContinuousWord { get; } = continuousWord;
    public Word? ObjectSpecifierWord { get; } = objectSpecifierWord;

    public VerbMood Mood => MoodWord.String.StringToVerbMood();
    public VerbTense Tense => TenseWord?.String.StringToVerbTense() ?? VerbTense.Present;
    public bool Continuous => ContinuousWord is not null;
}
