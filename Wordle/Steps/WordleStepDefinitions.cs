using TechTalk.SpecFlow;
using Wordle.Specs.Drivers;
using Wordle.Specs.PageObjects;
using Wordle.Specs.Services;
using System.Threading;
using NUnit.Framework;

namespace Wordle.Specs.Steps;

[Binding]
public sealed class WordleStepDefinitions
{
    //Page Object for Wordle
    private readonly WordlePageObject _wordlePageObject;
    private int attempt = 0;
    private bool done = false;

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

    [When(@"up to six words attempted")]
    public void WhenUpToSixWordsAttempted()
    {
        Solver solver = new Solver();
        while (!done)
        {
            var word = solver.getWord();
            attempt++;
            _wordlePageObject.enterWord(word);
            string[] evaluation =_wordlePageObject.wordEvaluation(attempt);
            done = solver.processEvaluation(evaluation) || attempt == 6;
        }
    }

    [Then(@"correct word found")]
    public void ThenCorrectWordFound()
    {
        Assert.IsTrue(done);
        Assert.LessOrEqual(attempt, 6);
        Thread.Sleep(60000);  //Allow time to view reult
    }

}