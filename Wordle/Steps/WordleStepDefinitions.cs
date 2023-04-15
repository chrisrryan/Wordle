using FluentAssertions;
using System.IO;
using System;
using TechTalk.SpecFlow;
using Wordle.Specs.Drivers;
using Wordle.Specs.PageObjects;
using System.Collections.Generic;
using System.Linq;

namespace Wordle.Specs.Steps
{
    [Binding]
    public sealed class WordleStepDefinitions
    {
        //Page Object for Wordle
        private readonly WordlePageObject _wordlePageObject;
        private List<string> _allWords;

        public WordleStepDefinitions(BrowserDriver browserDriver)
        {
            _wordlePageObject = new WordlePageObject(browserDriver.Current);
        }


        [Given(@"the Wordle home page is displayed")]
        public void GivenTheWordleHomePageIsDisplayed()
        {
            _wordlePageObject.EnsureWordleIsOpenAndReset();
        }

        [Given(@"the word list is loaded")]
        public void GivenTheWordListIsLoaded()
        {
            var fileStream = new FileStream(".\\Data\\Words.txt", FileMode.Open, FileAccess.Read);

            using (var reader = new StreamReader(fileStream))
            {
                var wordString = reader.ReadToEnd();
                _allWords = wordString.Split(new[] { '\n' }, StringSplitOptions.None).ToList();
            }
        }

        [Given(@"board reset")]
        public void GivenBoardReset()
        {

            foreach (var word in _allWords)
            {
                Console.WriteLine(word);
            };
            throw new PendingStepException();
        }
    }
}
