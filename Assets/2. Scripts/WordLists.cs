using UnityEngine;
using System.Collections.Generic;

public class WordLists
{
    public WordLists(List<Verb> verbs, List<Adjective> adjectives, 
        List<Noun> nouns, List<Expression> expressions, List<Adverb> adverbs, List<Grammer> grammers, List<KoSoADo> koSoADo)
    {
        Verbs = verbs;
        Adjectives = adjectives;
        Nouns = nouns;
        Expressions = expressions;
        Adverbs = adverbs;
        Grammers = grammers;
        KoSoADos = koSoADo;
    }

    public List<Grammer> Grammers;
    public List<Verb> Verbs;
    public List<Adjective> Adjectives;
    public List<Noun> Nouns;
    public List<Adverb> Adverbs;
    public List<Expression> Expressions;
    public List<KoSoADo> KoSoADos;
}
