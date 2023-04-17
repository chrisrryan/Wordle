@Wordle
Feature: Wordle
    
    Scenario: Solve Wordle
        Given the Wordle home page is displayed
        When up to six words attempted
        Then correct word found
