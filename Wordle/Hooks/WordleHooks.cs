using TechTalk.SpecFlow;
using Wordle.Specs.Drivers;
using Wordle.Specs.PageObjects;

namespace Wordle.Specs.Hooks
{
    /// <summary>
    /// Calculator related hooks
    /// </summary>
    [Binding]
    public class WordleHooks
    {
        ///<summary>
        ///  Reset the calculator before each scenario tagged with "Calculator"
        /// </summary>
        [BeforeScenario("Wordle")]
        public static void BeforeScenario(BrowserDriver browserDriver)
        {
            var wordlePageObject = new WordlePageObject(browserDriver.Current);
        }
    }
}