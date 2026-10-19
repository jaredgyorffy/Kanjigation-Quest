using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "DataSource", menuName = "ScriptableObject/DataSource")]
public class DataSource : ScriptableObject
{
    [Button]
    public void FormatData()
    {
        AddAnswersNouns();
        AddAnswersVerbs();
        AddAnswersAdjectives();
        AddAnswersAdverbs();
        AddAnswersExpressions();
        AddAnswersReading();
    }

    public void AddAnswersNouns()
    {
        foreach (Noun noun in VocabData.NounList)
        {
            noun.Meaning.Clear();
            List<string> items = noun.MeaningFull
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            noun.Meaning.AddRange(items);
        }
    }

    public void AddAnswersVerbs()
    {
        foreach (Verb verb in VocabData.VerbList)
        {
            verb.Meaning.Clear();
            List<string> items = verb.MeaningFull
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            verb.Meaning.AddRange(items);
        }
    }

    public void AddAnswersAdjectives()
    {
        foreach (Adjective adjectives in VocabData.AdjectiveList)
        {
            adjectives.Meaning.Clear();
            List<string> items = adjectives.MeaningFull
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            adjectives.Meaning.AddRange(items);
        }
    }

    public void AddAnswersExpressions()
    {
        foreach (Expression expression in VocabData.ExpressionList)
        {
            expression.Meaning.Clear();
            List<string> items = expression.MeaningFull
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            expression.Meaning.AddRange(items);
        }
    }

    public void AddAnswersAdverbs()
    {
        foreach (Adverb adverb in VocabData.AdverbList)
        {
            adverb.Meaning.Clear();
            List<string> items = adverb.MeaningFull
            .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
            adverb.Meaning.AddRange(items);
        }
    }

    [Button]
    public void PopulateGrammerExamples()
    {
        VocabData.GrammerList.Clear();
        foreach (GrammerExample example in VocabData.GrammerExamples)
        {
            bool containsKey = false;
            foreach (var grammer in VocabData.GrammerList)
            {
                if (grammer.Key == example.Key)
                {
                    grammer.Examples.Add(example);
                    containsKey = true;
                }
            }

            if (containsKey)
            {
                continue;
            }
            Grammer newGrammer = new Grammer(example.Answer, example.Answer, example.Key);
            newGrammer.Examples = new();
            newGrammer.Examples.Add(example);
            VocabData.GrammerList.Add(newGrammer);
        }
    }

    private void AddAnswersReading()
    {

        List<ReadingQuestion> answers = new List<ReadingQuestion>(VocabData.ReadingList);
        VocabData.ReadingList.Clear();
        foreach (ReadingQuestion question in answers)
        {
            bool containsKey = false;
            foreach (var answer in VocabData.ReadingList)
            {
                if (question.Key == answer.Key)
                {
                    answer.Answers.Add(question.Answer);
                    containsKey = true;
                }
            }

            if (containsKey)
            {
                continue;
            }
            ReadingQuestion NewReading = new ReadingQuestion(question.Kana, question.Answer, question.Key);
            NewReading.Answers.Add(question.Answer);
            VocabData.ReadingList.Add(NewReading);
        }
    }

    [field: SerializeField] public string SourceName { get; private set; }
    [field: SerializeField] public List<QuestionType> QuestionTypes;
    [field: SerializeField] public VocabData VocabData;
}