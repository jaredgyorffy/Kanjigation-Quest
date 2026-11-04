using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class QuestionStatistics
{
    public string questionId;
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

    // Historical difficulty, from 0 to 100.
    public float BaseDifficulty =>
        TotalAttempts > 0
            ? (float)incorrectCount / TotalAttempts * 100f
            : 50f;

    // Difficulty adjusted by recent performance.
    public int Difficulty
    {
        get
        {
            if (TotalAttempts == 0)
                return 50;

            float baseDifficulty = BaseDifficulty;

            // Each consecutive correct answer reduces
            // difficulty by 10 points.
            float streakAdjustment =
                consecutiveIncorrect * 10f -
                consecutiveCorrect * 10f;

            return Mathf.RoundToInt(Mathf.Clamp(
                baseDifficulty + streakAdjustment,
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
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime result))
            {
                return result.ToUniversalTime();
            }

            return null;
        }
    }
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
            "userData.json"
        );
    
    private static void LogPath()
    {
        Debug.Log(Path.Combine(
            Application.persistentDataPath,
            "userData.json"));
    }
    private static void EnsureLoaded()
    {
        if (statistics != null)
            return;
        LogPath();
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

    public static void RecordAnswer(
        string questionId,
        bool correct)
    {
        QuestionStatistics data = Get(questionId);

        if (correct)
            data.correctCount++;
        else
            data.incorrectCount++;

        data.lastEncounteredUtc =
            DateTime.UtcNow.ToString("O");

        Save();
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
