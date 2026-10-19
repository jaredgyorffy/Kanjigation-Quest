using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ReadingQuestion
{
    public ReadingQuestion(string kana, string answer, string key)
    {
        Kana = kana;
        Answer = answer;
        Key = key;
    }
    [field: SerializeField] public string Kana;
    [field: SerializeField] public string Answer;
    [field: SerializeField] public string Key;
    [field: SerializeField] public List<string> Answers = new();
}
