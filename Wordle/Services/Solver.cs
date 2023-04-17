using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Wordle.Specs.Services;
public class Solver
{
    private int _attempts;
    private List<string> _allWords;
    private string[] _possibles = new string[5];
    private Random random = new Random();
    private string _latestWord;

    public Solver()
    {
        // Import the Wordle words list
        var fileStream = new FileStream(".\\Data\\Words.txt", FileMode.Open, FileAccess.Read);
        using (var reader = new StreamReader(fileStream))
        {
            var wordString = reader.ReadToEnd();
            _allWords = wordString.Split(new[] { '\n' }, StringSplitOptions.None).ToList();
        }
        //Of each of the five letters, _possibles defines what letters could be in that position.
        //As feedback from Wordle comes in, possibles for each letter position are removed.
        _possibles = Array.ConvertAll(_possibles, p => "abcdefghijklmnopqrstuvwxyz");
    }

    public bool ProcessEvaluation(String[] evaluation)
    {
        /*From Wordle's colour coding - Page Objects supplies the as the evaluation array :
         Green (correct) - right letter in right position
         Yellow (present) - letter is present in word but not in that position
         Grey (absent) - the letter is not present in the word
         */
        
        var done = evaluation.All(x => x == "correct") || _attempts == 6;
        if (!done)
        {
            var letters = _latestWord.ToCharArray();
            for (int i = 0; i < 5; i++)
            {
                switch (evaluation[i])
                {
                    case "absent":
                        // Duplicate letter one present/correct and one absent.
                        bool repeatingLetterTooMany = false;
                        //Is this letter repeated in the word?
                        if (_latestWord.Count(x => x == letters[i]) >= 2)
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
                            RemoveLetterFromColumn(letters[i], i);
                        }
                        else
                        {
                            RemoveLetterFromAll(letters[i]);
                        }
                        break;
                    case "present":
                        MustHaveLetter(letters[i]);
                        RemoveLetterFromColumn(letters[i], i);
                        break;
                    case "correct":
                        MustHaveLetter(letters[i]);
                        SetLetterAsFound(letters[i], i);
                        break;
                }
            }
        }
        return done;
    }

    public void SetLetterAsFound(char letter, int index)
    {
        _possibles[index] = letter.ToString();
    }

    private void RemoveLetterFromAll(char letter)
    {
        for (int i = 0; i < 5; i++)
        {
            RemoveLetterFromColumn(letter, i);
        }
    }

    private void RemoveLetterFromColumn(char letter, int index)
    {
        _possibles[index] = _possibles[index].Replace(letter.ToString(), "");
    }

    private void MustHaveLetter(char letter)
    {
        _allWords = _allWords.Where(w => w.Contains(letter)).ToList();
    }

    public string GetWord()
    {
        _attempts++;

        if (_attempts ==  1)
            _latestWord = "stare";  //Reasonable first guess?
        else
        {
            //Use the possibles letter array to build a regular expression with which to then remove words which aren't 
            //potential solutions.
            var viableRegEx = ""; 
            for (int i = 0; i < 5; i++)
                viableRegEx += '[' + _possibles[i] + ']';

            //Using LINQ to filter the possible words using the regular expression above.
            var wordFilter = new Regex(viableRegEx);
            _allWords = _allWords.Where(f => wordFilter.IsMatch(f)).ToList();
            
            //From the remaining list select a word at random.
            //This could be improved with Information Theory. 
            _latestWord = _allWords[random.Next(_allWords.Count)];
        }
        return _latestWord;
    }
}