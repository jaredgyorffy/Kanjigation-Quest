using UnityEngine;
using System.Collections.Generic;

public class WordLists
{
    public WordLists(List<Verb> verbs, List<Adjective> adjectives, 
        List<Noun> nouns, List<Expression> expressions, List<Adverb> adverbs, List<Grammer> grammers, List<KoSoADo> koSoADo, List<ReadingQuestion> reading)
    {
        Verbs = verbs;
        Adjectives = adjectives;
        Nouns = nouns;
        Expressions = expressions;
        Adverbs = adverbs;
        Grammers = grammers;
        KoSoADos = koSoADo;
        Reading = reading;
    }

    public List<Grammer> Grammers;
    public List<Verb> Verbs;
    public List<Adjective> Adjectives;
    public List<Noun> Nouns;
    public List<Adverb> Adverbs;
    public List<Expression> Expressions;
    public List<ReadingQuestion> Reading;
    public List<KoSoADo> KoSoADos;
}
