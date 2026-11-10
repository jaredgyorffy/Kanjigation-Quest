using System;
using System.Collections.Generic;
using UnityEngine;

public static class QuestionSelectionUtility
{
    [Serializable]
    public class QuestionOptions
    {
        public QuestionSelectionMode Mode = QuestionSelectionMode.Difficulty;
        public int Difficulty = 50;
        public int Tolerance = 15;
        public QuestionOptions(QuestionSelectionMode mode, int difficulty, int tolerance)
        {
            Mode = mode;
            Difficulty = difficulty;
            Tolerance = tolerance;
        }
    }

    public const int TOLERANCE = 15;
    public static QuestionOptions DefaultOptions()
    {
        return new QuestionOptions(QuestionSelectionMode.Difficulty, 50, TOLERANCE);
    }
    public static QuestionOptions GetOptionsByDifficulty(QuestionDifficulty difficulty)
    {
        return difficulty switch
        {
            QuestionDifficulty.New => new QuestionOptions(QuestionSelectionMode.UnseenWord, 0, TOLERANCE),
            QuestionDifficulty.Trivial => new QuestionOptions(QuestionSelectionMode.Difficulty, 25, TOLERANCE),
            QuestionDifficulty.Medium => new QuestionOptions(QuestionSelectionMode.Difficulty, 50, TOLERANCE),
            QuestionDifficulty.Hard => new QuestionOptions(QuestionSelectionMode.Difficulty, 75, TOLERANCE),
            QuestionDifficulty.Impossible => new QuestionOptions(QuestionSelectionMode.Difficulty, 90, TOLERANCE),
            _ => DefaultOptions()
        };
    }

    public enum QuestionSelectionMode
    {
        Difficulty,
        UnseenWord,
    }
    public static T SelectWordByOptions<T>(
       IEnumerable<T> words,
       QuestionCategory category,
       QuestionOptions options)
       where T : class, IWord
    {
        return SelectWordByDifficulty(
            words,
            category,
            options.Difficulty,
            options.Tolerance,
            options.Mode
            );
    }

    public static T SelectWordByDifficulty<T>(
       IEnumerable<T> words,
       QuestionCategory category,
       int targetDifficulty = 50,
       int difficultyTolerance = 15,
       QuestionSelectionMode mode =
           QuestionSelectionMode.Difficulty)
       where T : class, IWord
    {
        if (words == null)
            return null;

        targetDifficulty = Mathf.Clamp(
            targetDifficulty, 0, 100);

        difficultyTolerance = Mathf.Clamp(
            difficultyTolerance, 0, 100);

        List<T> candidates = new();
        List<float> weights = new();

        int nearestDifference = int.MaxValue;

        // First pass: find eligible words.
        foreach (T word in words)
        {
            if (word == null ||
                string.IsNullOrEmpty(word.GUID))
                continue;

            QuestionStatistics stats =
                QuestionStatisticsManager.Get(word.GUID);

            AnswerStatistics answerStats =
                stats.Get(category);

            // Select a completely unseen word.
            if (mode == QuestionSelectionMode.UnseenWord)
            {
                if (stats.TotalAttempts == 0)
                    candidates.Add(word);

                continue;
            }

            // Difficulty selection requires previous attempts.
            if (answerStats.TotalAttempts == 0)
                continue;

            int difference = Mathf.Abs(
                answerStats.Difficulty - targetDifficulty);

            nearestDifference = Mathf.Min(
                nearestDifference, difference);

            if (difference <= difficultyTolerance)
                candidates.Add(word);
        }

        // Unseen selection uses uniform randomness.
        if (mode != QuestionSelectionMode.Difficulty)
            return RandomCandidate(candidates);

        // No questions within tolerance:
        // fall back to the closest available difficulty.
        if (candidates.Count == 0)
        {
            if (nearestDifference == int.MaxValue)
                return null;

            foreach (T word in words)
            {
                if (word == null ||
                    string.IsNullOrEmpty(word.GUID))
                    continue;

                AnswerStatistics answerStats =
                    QuestionStatisticsManager.Get(
                        word.GUID, category);

                if (answerStats.TotalAttempts == 0)
                    continue;

                int difference = Mathf.Abs(
                    answerStats.Difficulty - targetDifficulty);

                if (difference == nearestDifference)
                    candidates.Add(word);
            }
        }

        // Calculate selection weights.
        foreach (T word in candidates)
        {
            AnswerStatistics answerStats =
                QuestionStatisticsManager.Get(
                    word.GUID, category);

            weights.Add(CalculateWeight(
                answerStats, targetDifficulty));
        }

        return WeightedRandom(candidates, weights);
    }

    /// <summary>
    /// Calculates the selection weight using
    /// difficulty similarity and elapsed time.
    /// </summary>
    private static float CalculateWeight(
        AnswerStatistics stats,
        int targetDifficulty)
    {
        // Higher weight when difficulty is closer.
        float difference = Mathf.Abs(
            stats.Difficulty - targetDifficulty);

        float difficultyWeight =
            1f / Mathf.Pow(1f + difference / 10f, 2f);

        // Higher weight when not encountered recently.
        float recencyWeight = 1f;

        if (stats.LastEncountered.HasValue)
        {
            double hours = (
                DateTime.UtcNow -
                stats.LastEncountered.Value
            ).TotalHours;

            float elapsedHours =
                Mathf.Max(0f, (float)hours);

            // Just seen: 15% weight.
            // After 24 hours: 100% weight.
            recencyWeight = Mathf.Lerp(
                0.15f,
                1f,
                Mathf.Clamp01(elapsedHours / 24f)
            );
        }

        return difficultyWeight * recencyWeight;
    }

    /// <summary>
    /// Returns a uniformly random candidate.
    /// </summary>
    private static T RandomCandidate<T>(
        List<T> candidates)
        where T : class, IWord
    {
        if (candidates.Count == 0)
            return null;

        return candidates[
            UnityEngine.Random.Range(0, candidates.Count)
        ];
    }

    /// <summary>
    /// Returns a candidate based on selection weights.
    /// </summary>
    private static T WeightedRandom<T>(
        List<T> candidates,
        List<float> weights)
        where T : class, IWord
    {
        if (candidates.Count == 0)
            return null;

        float totalWeight = 0f;

        foreach (float weight in weights)
            totalWeight += weight;

        if (totalWeight <= 0f)
            return RandomCandidate(candidates);

        float random =
            UnityEngine.Random.value * totalWeight;

        for (int i = 0; i < candidates.Count; i++)
        {
            random -= weights[i];

            if (random <= 0f)
                return candidates[i];
        }

        return candidates[candidates.Count - 1];
    }
}
