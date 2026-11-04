using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Grammer
{
    public Grammer()
    {
    }
    public Grammer(string kana, string kanji, string Key, string GUID)
    {
        this.Kana = kana;
        this.Kanji = kanji;
        this.Key = Key;
        this.GUID = GUID;
    }
    [field: SerializeField] public string Kana;
    [field: SerializeField] public string Kanji;
    [field: SerializeField] public string Key;
    [field: SerializeField] public string Explanation;
    [field: SerializeField] public string GUID;
    [field: SerializeField] public List<GrammerExample> Examples;
}

[Serializable]
public class GrammerExample
{
    public string Question;
    public string Answer;
    public string Translation;
    public string Hint;
    public string Key;
    public string GUID;
}