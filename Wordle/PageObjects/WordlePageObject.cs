using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Wordle.Specs.PageObjects;

/// <summary>
/// Wordle Page Object
/// </summary>
public class WordlePageObject
{
    //The URL of the calculator to be opened in the browser
    private const string WordleUrl = "https://www.nytimes.com/games/wordle/index.html";

    //The Selenium web driver to automate the browser
    private readonly IWebDriver _webDriver;
        
    //The default wait time in seconds for wait.Until
    public const int DefaultWaitInSeconds = 5;

    public WordlePageObject(IWebDriver webDriver)
    {
        _webDriver = webDriver;
    }

    //Finding elements
    private IWebElement GDPRelement => _webDriver.FindElement(By.Id("pz-gdpr-btn-closex"));
    private IWebElement HowToPlayElement => _webDriver.FindElement(By.XPath("//button[contains(@aria-label, 'Close')]"));
    public IWebElement KeyboardElement => _webDriver.FindElement(By.XPath("//div[contains(@aria-label, 'Keyboard')]"));


    //Finding elements by ID
    private IWebElement FirstNumberElement => _webDriver.FindElement(By.Id("first-number"));
    private IWebElement SecondNumberElement => _webDriver.FindElement(By.Id("second-number"));
    private IWebElement AddButtonElement => _webDriver.FindElement(By.Id("add-button"));
    private IWebElement ResultElement => _webDriver.FindElement(By.Id("result"));
    private IWebElement ResetButtonElement => _webDriver.FindElement(By.Id("reset-button"));

    public void ClickGDPR()
    {
        GDPRelement.Click();
    }

    public void ClickHowToPlay()
    {
        HowToPlayElement.Click();
    }

    public bool enterWord(string word)
    {
        for (int i = 0; i < 5; i++) clickLetter(word[i]);
        clickLetter('↵'); // Enter key
            
        // Wordle doesn't reveal word results immediately. Give it time to do its thing.
        Thread.Sleep(2000);
        return true;
    }

    private void clickLetter(char letter)
    {
        KeyboardElement.FindElement(By.CssSelector(String.Format("button[data-key='{0}']", letter))).Click();
    }

    public string[] wordEvaluation(int attempt)
    {
        String[] evaluation = new String[5];
        for (int i = 0; i < 5; i++)
            evaluation[i] = letterEvaluation(attempt, i + 1);
        return evaluation;
    }

    private String letterEvaluation(int rowIndex, int tileIndex)
    {
        String tileXPath = "//div[contains(@aria-label, 'Row " + rowIndex + "')]/div[" + tileIndex + "]/div";
        IWebElement tile = _webDriver.FindElement(By.XPath(tileXPath));
        return tile.GetDomAttribute("data-state");
    }

    public void EnsureWordleIsOpenAndReset()
    {
        //Open the calculator page in the browser if not opened yet
        if (_webDriver.Url != WordleUrl)
        {
            _webDriver.Url = WordleUrl;
        }
        //Otherwise reset the calculator by clicking the reset button
        else
        {
            //Click the rest button
            ResetButtonElement.Click();

            //Wait until the result is empty again
            WaitForEmptyResult();
        }
    }

    public string WaitForNonEmptyResult()
    {
        //Wait for the result to be not empty
        return WaitUntil(
            () => ResultElement.GetAttribute("value"),
            result => !string.IsNullOrEmpty(result));
    }

    public string WaitForEmptyResult()
    {
        //Wait for the result to be empty
        return WaitUntil(
            () => ResultElement.GetAttribute("value"),
            result => result == string.Empty);
    }

    /// <summary>
    /// Helper method to wait until the expected result is available on the UI
    /// </summary>
    /// <typeparam name="T">The type of result to retrieve</typeparam>
    /// <param name="getResult">The function to poll the result from the UI</param>
    /// <param name="isResultAccepted">The function to decide if the polled result is accepted</param>
    /// <returns>An accepted result returned from the UI. If the UI does not return an accepted result within the timeout an exception is thrown.</returns>
    private T WaitUntil<T>(Func<T> getResult, Func<T, bool> isResultAccepted) where T: class
    {
        var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(DefaultWaitInSeconds));
        return wait.Until(driver =>
        {
            var result = getResult();
            if (!isResultAccepted(result))
                return default;

            return result;
        });

    }
}