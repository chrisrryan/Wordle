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
    private int _attempt = 0;
    private bool _done = false;

    public WordleStepDefinitions(BrowserDriver browserDriver)
    {
        _wordlePageObject = new WordlePageObject(browserDriver.Current);
    }
    
    [Given(@"the Wordle home page is displayed")]
    public void GivenTheWordleHomePageIsDisplayed()
    {
        _wordlePageObject.EnsureWordleIsOpenAndReset();
        _wordlePageObject.ClickGdpr();
        _wordlePageObject.ClickHowToPlay();
    }

    [When(@"up to six words attempted")]
    public void WhenUpToSixWordsAttempted()
    {
        Solver solver = new Solver();
        while (!_done)
        {
            var word = solver.GetWord();
            _attempt++;
            _wordlePageObject.EnterWord(word);
            string[] evaluation =_wordlePageObject.WordEvaluation(_attempt);
            _done = solver.ProcessEvaluation(evaluation) || _attempt == 6;
        }
    }

    [Then(@"correct word found")]
    public void ThenCorrectWordFound()
    {
        Assert.IsTrue(_done);
        Assert.LessOrEqual(_attempt, 6);
        Thread.Sleep(60000);  //Allow time to view result
    }
}