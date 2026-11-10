using System;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTest : MonoBehaviour, IQuiz
{
    QuizMenu quizMenu;
    public event Action<bool, string> AnswerSubmitted;

    private QuestionType questionType;
    private Question currentQuestion;

    public WordLists WordLists;

    private List<Question> askedQuestions = new();

    private bool confirmAnswer = false;

    private bool PracticeMode = false;
    private bool studyNewItems = false;
    private bool useKanaKeyboard;

    public bool Initialized { get; private set; }

    public void SetQuestionType(QuestionType types)
    {
        questionType = types;

        if (useKanaKeyboard == false)
        {
            return;
        }
        if (types.UseKanaKeyboard)
        {
            quizMenu.SetKanaKeyboard(true);
        }
        else
        {
            quizMenu.SetKanaKeyboard(false);
        }
    }

    public void InitializeQuiz(QuizConfiguration config, QuizMenu quizmenu)
    {
        WordLists = new WordLists(config.Verbs, config.Adjectives, config.Nouns, config.Expressions, config.Adverbs, config.Grammers, config.KoSOADo, config.Readings);
        quizMenu = quizmenu;
        askedQuestions = new();
        SetInformationText("");
        useKanaKeyboard = config.UseKanaKeyboard;
        quizMenu.SetKanaKeyboard(useKanaKeyboard);
        PracticeMode = config.PracticeMode;
        studyNewItems = config.StudyNewItems;
    }

    public void SetInformationText(string text)
    {
        quizMenu.SetInformationText(text);
    }

    public (bool, string) OnPressSubmit(string answer)
    {
        ToggleHint(false);

        if (ShouldConfirmAnswer())
        {
            if (CheckAnswer(answer))
            {
                SetInformationText("Correct");
                confirmAnswer = true;
                return (true, answer);
            }
            else
            {
                SetInformationText("Wrong! Try again?");
                confirmAnswer = true;
                return (false, "");
            }
        }
        else
        {
            confirmAnswer = false;
            if (CheckAnswer(answer))
            {
                SetInformationText("");
                AnswerSubmitted?.Invoke(true, answer);
                if (!PracticeMode && !string.IsNullOrEmpty(currentQuestion.GUID))
                {
                    QuestionStatisticsManager.RecordAnswer(currentQuestion.GUID, QuestionCategory.Vocab, true);
                }

                return (true, answer);
            }
            else
            {
                SetInformationText("");
                AnswerSubmitted?.Invoke(false, "");
                if (!PracticeMode && !string.IsNullOrEmpty(currentQuestion.GUID))
                {
                    QuestionStatisticsManager.RecordAnswer(currentQuestion.GUID, QuestionCategory.Vocab, false);
                }
                return (false, GetAnswer());
            }
        }
    }

    public bool CheckAnswer(string userAnswer)
    {
        return QuizUtility.CheckAnswer(userAnswer, currentQuestion);
    }

    private string GetAnswer()
    {
        return QuizUtility.GetAnswer(currentQuestion.Answers);
    }

    private bool ShouldConfirmAnswer() => PracticeMode && confirmAnswer == false;

    public void PrepareNextQuestion(QuestionDifficulty questionDifficulty = QuestionDifficulty.Medium)
    {
        askedQuestions.Add(currentQuestion);
        Question question = GetQuestion(questionDifficulty);
        currentQuestion = question;

        if (question.Category != QuestionCategory.KoSoADo)
        {
            SetKana(question.Word);
        }

        if (question.Category == QuestionCategory.Reading)
        {
            quizMenu.SetTextQuestion(question.QuestionText);
            quizMenu.SetMenuType(MenuType.MultipleChoice);
            quizMenu.SetMultipleChoiceQuestions(question.IncorrectAnswers, question.Answers[0]);
        }
        else
        {
            quizMenu.SetMenuType(MenuType.TextAnswer);
            quizMenu.SetTextQuestion(question.QuestionText);
        }

    }

    private void SetKana(string kana, string Kanji)
    {
        quizMenu.SetKana(kana, Kanji);
    }

    private void SetKana(IWord word)
    {
        quizMenu.SetKana(word.Kana, word.Kanji);
    }

    private Question GetQuestion(QuestionDifficulty questionDifficulty = QuestionDifficulty.Medium)
    {
        switch (questionType.Category)
        {
        case QuestionCategory.Conjugation:
            return GetConjugationQuesiton();
        case QuestionCategory.Grammar:
            return GetGrammerQuestion();
        case QuestionCategory.Expression:
            return GetExpressionQuestion();
        case QuestionCategory.Vocab:
            return GetVocabQuestion(questionDifficulty);
        case QuestionCategory.Numbers:
            int maxRange = 99;
            int.TryParse(questionType.Data, out maxRange);
            return GetNumbersQuestion(maxRange);
        case QuestionCategory.KoSoADo:
            return GetKoSoADoQuestion();
        case QuestionCategory.Reading:
            return GetReadingQuestion();
        default:
            Debug.LogError($"invalid Question Category {questionType.Category}");
            return null;
        }
    }

    private Question GetKoSoADoQuestion()
    {
        (Question question, Texture texture) data = QuizUtility.GetKoSOADoQuestion(WordLists);
        quizMenu.SetQuestionImage(data.texture);

        return data.question;
    }

    private Question GetNumbersQuestion(int maxRange)
    {
        int number = 0;
        if (maxRange > 99)
        {
            number = UnityEngine.Random.Range(99, maxRange);
        }
        else
        {
            number = UnityEngine.Random.Range(0, maxRange);
        }

        return QuizUtility.GetNumbersQuestion(number);
    }

    private Question GetVocabQuestion(QuestionDifficulty questionDifficulty = QuestionDifficulty.Medium)
    {
        if (studyNewItems)
        {
            return QuizUtility.GetVocabQuestion(ref WordLists, QuestionDifficulty.New);
        }
        else
        {
            return QuizUtility.GetVocabQuestion(ref WordLists, questionDifficulty);
        }
            
    }

    private Question GetExpressionQuestion()
    {
        IWord word = QuizUtility.GetRandomWordByType(ref WordLists, WordType.Expression);
        if (word == null)
        {
            Debug.LogError("Ran out of expressions");
        }
        return QuizUtility.GetExpressionQuestion(word);
    }

    private Question GetConjugationQuesiton()
    {
        List<string> answers = new List<string>();

        WordType wordType = QuizUtility.GetRandomWordType(questionType.ConjugationTypes, WordLists);
        ConjugationType form = questionType.ConjugationTypes.GetConjugationTypeByWordType(wordType);
        IWord word = QuizUtility.GetRandomWordByType(ref WordLists, wordType);

        if (word == null)
        {
            Debug.LogError($"Ran out of {wordType.ToString()}");
        }

        return QuizUtility.GetConjugationQuestion(wordType, form, word);
    }

    private Question GetGrammerQuestion()
    {
        return QuizUtility.GetRandomGrammer(WordLists);
    }

    private Question GetReadingQuestion()
    {
        Question reading = QuizUtility.GetRandomReading(WordLists);
        return QuizUtility.GetRandomReading(WordLists);
    }

    public void ToggleHint(bool hintVisible)
    {
        if (hintVisible)
        {
            SetInformationText("");
        }
        else
        {
            SetInformationText(currentQuestion.Hint);
        }
    }
}
