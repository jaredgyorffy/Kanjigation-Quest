using UnityEngine;
using System.Collections.Generic;
public class Question
{
    public Question(string QuestionText, List<string> Answers, IWord word, QuestionCategory category, string hint, string guid, List<string> incorrectAnswers = null)
    {
        this.QuestionText = QuestionText;
        this.Category = category;
        this.Answers = Answers;
        this.Hint = hint;
        this.Word = word;
        this.IncorrectAnswers = incorrectAnswers;
        this.GUID = guid;
    }
    public IWord Word { get; private set; }
    public QuestionCategory Category { get; private set; }
    public string QuestionText { get; private set; }
    public List<string> Answers { get; private set; }
    public List<string> IncorrectAnswers { get; private set; }
    public string Hint { get; private set; }
    public string GUID { get; private set; }
}
