
using System.Collections.Generic;
public static class NumberTranslator
{
    public static List<string> GetOnesColumn(int number)
    {
        List<string> translation = new();
        if (number < 10)
        {
            switch (number)
            {
            case 0:
                translation.Add("れい");
                break;
            case 1:
                translation.Add("いち");
                break;
            case 2:
                translation.Add("に");
                break;
            case 3:
                translation.Add("さん");
                break;
            case 4:
                translation.Add("よん");
                translation.Add("よ");
                translation.Add("し");
                break;
            case 5:
                translation.Add("ご");
                break;
            case 6:
                translation.Add("ろく");
                break;
            case 7:
                translation.Add("しち");
                translation.Add("なな");
                break;
            case 8:
                translation.Add("はち");
                break;
            case 9:
                translation.Add("きゅう");
                translation.Add("く");
                break;
            default:
                translation.Add("");
                return translation;
            }
            return translation;
        }
        else
        {
            translation.Add("Error");
            return translation;
        }
    }

    public static List<string> GetTensColumn(int number)
    {
        List<string> translation = new();
        int tensRemainder = number % 10;
        int tens = number / 10;

        switch (tens)
        {
        case 1:
            translation.Add("じゅう");
            break;
        case 2:
            translation.Add("にじゅう");
            break;
        case 3:
            translation.Add("さんじゅう");
            break;
        case 4:
            translation.Add("よんじゅう");
            break;
        case 5:
            translation.Add("ごじゅう");
            break;
        case 6:
            translation.Add("ろくじゅう");
            break;
        case 7:
            translation.Add("しちじゅう");
            translation.Add("ななじゅう");
            break;
        case 8:
            translation.Add("はちじゅう");
            break;
        case 9:
            translation.Add("きゅうじゅう");
            translation.Add("くじゅう");
            break;
        default:
            translation.Add("");
            return translation;
        }

        List<string> newStrings = new();

        if (tensRemainder > 0)
        {
            for (int i = 0; i < translation.Count; i++)
            {
                List<string> onesColumn = GetOnesColumn(tensRemainder);
                {
                    foreach (string ones in onesColumn)
                    {
                        string combinedWord = translation[i] + ones;
                        newStrings.Add(combinedWord);
                    }
                }
            }
            return newStrings;
        }
        return translation;
    }

    public static List<string> GetNumberTranslation(int number)
    {
        List<string> translation = new();
        if (number < 10)
        {
            return GetOnesColumn(number);
        }
        else if (number < 100)
        {
            return GetTensColumn(number);
        }
        else
        {
            int hundredRemainder = number % 100;
            int hundreds = number / 100;

            switch (hundreds)
            {
            case 1:
                translation.Add("ひゃく");
                break;
            case 2:
                translation.Add("にひゃく");
                break;
            case 3:
                translation.Add("さんびゃく");
                break;
            case 4:
                translation.Add("よんひゃく");
                break;
            case 5:
                translation.Add("ごひゃく");
                break;
            case 6:
                translation.Add("ろっぴゃく");
                break;
            case 7:
                translation.Add("しちひゃく");
                translation.Add("ななひゃく");
                break;
            case 8:
                translation.Add("はちぴゃく");
                break;
            case 9:
                translation.Add("きゅうひゃく");
                translation.Add("くひゃく");
                break;
            default:
                translation.Add("");
                return translation;
            }

            if (hundredRemainder == 0)
            {
                return translation;
            }

            List<string> newStrings = new();
            for (int i = 0; i < translation.Count; i++)
            {
                List<string> tensColumn = GetTensColumn(hundredRemainder);
                {
                    foreach (string tens in tensColumn)
                    {
                        string combinedWord = translation[i] + tens;
                        newStrings.Add(combinedWord);
                    }
                }
            }
            return newStrings;
        }
    }
}
