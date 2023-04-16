using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wordle.Specs.Services;

public class Solver
{
    public int attempts { get; set; }

    private List<string> _allWords;
    private string[] possibles = new string[5];
    private Random random = new Random();
    private string latestWord;

    public Solver()
    {
        // Import the Wordle words list
        var fileStream = new FileStream(".\\Data\\Words.txt", FileMode.Open, FileAccess.Read);
        using (var reader = new StreamReader(fileStream))
        {
            var wordString = reader.ReadToEnd();
            _allWords = wordString.Split(new[] { '\n' }, StringSplitOptions.None).ToList();
        }

        possibles = Array.ConvertAll(possibles, p => "abcdefghijklmnopqrstuvwxyz");
    }

    public bool processEvaluation(String[] evaluation)
    {
        bool done = evaluation.All(x => x == "correct") || attempts == 6;
        if (!done)
        {
            char[] letters = latestWord.ToCharArray();
            for (int i = 0; i < 5; i++)
            {
                switch (evaluation[i])
                {
                    case "absent":
                        // Duplicate letter one present/correct and one absent.
                        bool repeatingLetterTooMany = false;
                        //Is this letter repeated in the word?
                        if (latestWord.Count(x => x == letters[i]) >= 2)
                        {
                            // If so, check if any are flagged as present or correct.
                            for (int j = 0; j < 5; j++)
                            {
                                if (letters[j] == letters[i] && evaluation[j] != "absent")
                                    repeatingLetterTooMany = true;
                            }
                        }
                        if (repeatingLetterTooMany)
                        {
                            removeLetterFromColumn(letters[i], i);
                        }
                        else
                        {
                            removeLetterFromAll(letters[i]);
                        }
                        break;
                    case "present":
                        mustHaveLetter(letters[i]);
                        removeLetterFromColumn(letters[i], i);
                        break;
                    case "correct":
                        mustHaveLetter(letters[i]);
                        setLetterAsFound(letters[i], i);
                        break;
                }
            }
        }
        return done;
    }

    public void setLetterAsFound(char letter, int index)
    {
        possibles[index] = letter.ToString();
    }

    public void removeLetterFromAll(char letter)
    {
        for (int i = 0; i < 5; i++)
        {
            removeLetterFromColumn(letter, i);
        }
    }

    public void removeLetterFromColumn(char letter, int index)
    {
        possibles[index] = possibles[index].Replace(letter.ToString(), "");
    }

    public void mustHaveLetter(char letter)
    {
        _allWords = _allWords.Where(w => w.Contains(letter)).ToList();
    }

    public String getWord()
    {
        attempts++;

        switch (attempts)
        {
            case 1:
                latestWord = "adieu";
                break;
            default:
                String viableRegEx = "";
                for (int i = 0; i < 5; i++)
                    viableRegEx += '[' + possibles[i] + ']';

                var wordFilter = new Regex(viableRegEx);
                _allWords = _allWords.Where(f => wordFilter.IsMatch(f)).ToList();

                latestWord = _allWords[random.Next(_allWords.Count)];
                break;
        }
        return latestWord;
    }
}