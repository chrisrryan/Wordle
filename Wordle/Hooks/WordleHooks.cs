using TechTalk.SpecFlow;
using Wordle.Specs.Drivers;
using Wordle.Specs.PageObjects;

namespace Wordle.Specs.Hooks;

[Binding]
public class WordleHooks
{
    [BeforeScenario("Wordle")]
    public static void BeforeScenario(BrowserDriver browserDriver)
    {
        var wordlePageObject = new WordlePageObject(browserDriver.Current);
    }
}