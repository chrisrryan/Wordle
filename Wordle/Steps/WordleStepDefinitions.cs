using FluentAssertions;
using System.IO;
using System;
using TechTalk.SpecFlow;
using Wordle.Specs.Drivers;
using Wordle.Specs.PageObjects;
using System.Collections.Generic;
using System.Linq;
using Wordle.Specs.Services;

namespace Wordle.Specs.Steps;

[Binding]
public sealed class WordleStepDefinitions
{
    //Page Object for Wordle
    private readonly WordlePageObject _wordlePageObject;

    public WordleStepDefinitions(BrowserDriver browserDriver)
    {
        _wordlePageObject = new WordlePageObject(browserDriver.Current);
    }
    
    [Given(@"the Wordle home page is displayed")]
    public void GivenTheWordleHomePageIsDisplayed()
    {
        _wordlePageObject.EnsureWordleIsOpenAndReset();
        _wordlePageObject.ClickGDPR();
        _wordlePageObject.ClickHowToPlay();
    }

    [Given(@"make attempts")]
    public void GivenBoardReset()
    {
        Solver solver = new Solver();
        var attempt = 0;
        var done = false;

        while (!done)
        {
            var word = solver.getWord();
            attempt++;
            _wordlePageObject.enterWord(word);
            string[] evaluation =_wordlePageObject.wordEvaluation(attempt);
            done = solver.processEvaluation(evaluation) || attempt == 6;
        }
    }
}