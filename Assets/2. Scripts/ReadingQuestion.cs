using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReadingQuestion : IWord
{
    public WordType WordType => WordType.Reading;
    public ReadingQuestion()
    {

    }
    public ReadingQuestion(string Kana, string Meaning, string key)
    {
        this.kana = Kana;
        this.Answer = Meaning;
        this.Key = key;
    }

    public string Kanji => "";
    public string Kana => kana;
    [SerializeField] private string kana;
    public List<string> Meaning => GetMeaning();
    private List<string> GetMeaning()
    {
        List<string> meanings = new();
        meanings.Add(Answer);
        return meanings;
    }

    [field: SerializeField] public string Answer;
    [field: SerializeField] public string Key;
    [field: SerializeField] public List<string> Answers = new();
}
