
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

[Serializable]
public class AnswerStatistics
{
    public int correctCount;
    public int incorrectCount;

    public int consecutiveCorrect;
    public int consecutiveIncorrect;

    public string lastEncounteredUtc;

    public int TotalAttempts =>
        correctCount + incorrectCount;

    public float SuccessRate =>
        TotalAttempts > 0
            ? (float)correctCount / TotalAttempts
            : 0f;

    public float BaseDifficulty =>
        TotalAttempts > 0
            ? (float)incorrectCount / TotalAttempts * 100f
            : 50f;

    public int Difficulty
    {
        get
        {
            if (TotalAttempts == 0)
                return 50;

            float adjustment =
                consecutiveIncorrect * 10f -
                consecutiveCorrect * 10f;

            return Mathf.RoundToInt(Mathf.Clamp(
                BaseDifficulty + adjustment,
                0f,
                100f
            ));
        }
    }

    public DateTime? LastEncountered
    {
        get
        {
            if (DateTime.TryParse(
                lastEncounteredUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out DateTime result))
            {
                return result.ToUniversalTime();
            }

            return null;
        }
    }

    public void RecordAnswer(bool correct)
    {
        if (correct)
        {
            correctCount++;
            consecutiveCorrect++;
            consecutiveIncorrect = 0;
        }
        else
        {
            incorrectCount++;
            consecutiveIncorrect++;
            consecutiveCorrect = 0;
        }

        lastEncounteredUtc =
            DateTime.UtcNow.ToString("O");
    }
}

[Serializable]
public class QuestionStatistics
{
    public string questionId;

    public AnswerStatistics meaning = new();
    public AnswerStatistics reading = new();

    public AnswerStatistics Get(QuestionCategory type)
    {
        return type switch
        {
            QuestionCategory.Vocab => meaning,
            QuestionCategory.Reading => reading,
            _ => throw new ArgumentOutOfRangeException(
                nameof(type))
        };
    }

    public int TotalAttempts =>
        meaning.TotalAttempts + reading.TotalAttempts;

    // Combined difficulty for the vocabulary word.
    public int Difficulty =>
        Mathf.RoundToInt(
            (meaning.Difficulty + reading.Difficulty) / 2f
        );
}

[Serializable]
public class QuestionStatisticsSaveData
{
    public List<QuestionStatistics> questions = new();
}

public static class QuestionStatisticsManager
{
    private static Dictionary<string, QuestionStatistics>
        statistics;

    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "question_statistics.json"
        );

    private static void EnsureLoaded()
    {
        if (statistics == null)
            Load();
    }

    public static QuestionStatistics Get(string questionId)
    {
        if (string.IsNullOrEmpty(questionId))
            throw new ArgumentException("Invalid question ID.");

        EnsureLoaded();

        if (!statistics.TryGetValue(
            questionId, out QuestionStatistics data))
        {
            data = new QuestionStatistics
            {
                questionId = questionId
            };

            statistics.Add(questionId, data);
        }

        return data;
    }

    public static AnswerStatistics Get(
        string questionId,
        QuestionCategory type)
    {
        return Get(questionId).Get(type);
    }

    public static void RecordAnswer(
        string questionId,
        QuestionCategory type,
        bool correct)
    {
        AnswerStatistics data = Get(questionId, type);

        data.RecordAnswer(correct);

        Save();
    }

    public static int GetDifficulty(
        string questionId,
        QuestionCategory type)
    {
        return Get(questionId, type).Difficulty;
    }

    public static int GetDifficulty(string questionId)
    {
        return Get(questionId).Difficulty;
    }

    public static void Save()
    {
        EnsureLoaded();

        QuestionStatisticsSaveData saveData = new()
        {
            questions = new List<QuestionStatistics>(
                statistics.Values
            )
        };

        string json = JsonUtility.ToJson(saveData, true);

        Directory.CreateDirectory(
            Application.persistentDataPath
        );

        File.WriteAllText(SavePath, json);
    }

    public static void Load()
    {
        statistics = new Dictionary<string, QuestionStatistics>();

        if (!File.Exists(SavePath))
            return;

        try
        {
            string json = File.ReadAllText(SavePath);

            QuestionStatisticsSaveData saveData =
                JsonUtility.FromJson<QuestionStatisticsSaveData>(
                    json
                );

            if (saveData?.questions == null)
                return;

            foreach (QuestionStatistics data in saveData.questions)
            {
                if (data == null ||
                    string.IsNullOrEmpty(data.questionId))
                    continue;

                // Initialize missing data from older saves.
                data.meaning ??= new AnswerStatistics();
                data.reading ??= new AnswerStatistics();

                statistics[data.questionId] = data;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Failed to load question statistics: {e}"
            );
        }
    }

    public static void Reset(
        string questionId,
        QuestionCategory type)
    {
        QuestionStatistics data = Get(questionId);

        switch (type)
        {
        case QuestionCategory.Vocab:
            data.meaning = new AnswerStatistics();
            break;

        case QuestionCategory.Reading:
            data.reading = new AnswerStatistics();
            break;

        default:
            throw new ArgumentOutOfRangeException(
                nameof(type));
        }

        Save();
    }

    public static void Reset(string questionId)
    {
        EnsureLoaded();

        if (statistics.Remove(questionId))
            Save();
    }

    public static void ResetAll()
    {
        statistics = new Dictionary<string, QuestionStatistics>();
        Save();
    }
}
