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
        this.answer = Meaning;
        this.key = key;
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
    public string Answer => answer;
    [SerializeField] private string answer;
    public string Key => key;
    [SerializeField] private string key;
    public List<string> Answers => answers;
    [SerializeField] private List<string> answers = new();
}
