using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script allows for the List to be created within the HighScoreTable script.
public class ScoreList : MonoBehaviour {

    public string playerName;   // This will contain the players name for the high score table.
    public int score;           // This will contain the score for the high score table.

    // This function sets up the constructor for the List
    public ScoreList(string newName, int newScore) {
        playerName = newName;   // Set the playerName string to the first parametre in the constructor newName.
        score = newScore;       // Set the score int to the second parametre in the constructor newScore.
    }    
}

 