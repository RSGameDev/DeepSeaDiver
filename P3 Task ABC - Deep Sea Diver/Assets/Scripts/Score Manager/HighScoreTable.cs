using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

// This script creates the high score table and adds entries in a descending order of score.
public class HighScoreTable : MonoBehaviour {
    
    public static List<ScoreList> highscore = new List<ScoreList>();    // Initialise the list that keeps the high scores.
    public GameObject sceneControl;     // Reference to the scene controller GameObject.
    public Player player;               // Reference to the Player script.
    public Text inputField;             // Reference to the text field inside of the Input Field component. This will store the player input for the high score table.
    public Text[] nameText;             // Reference to a text array that will contain the names for the high score table.
    public Text[] scoreText;            // Reference to a text array that will contain the scores for the high score table.
    string tempName;                    // This stores the name inputted by the player via the InputField component.
    int playerScore;                    // This stores the score accessed through another script (Player script).


    void Start() {
        // Set up the table for the high score table.
        highscore.Add(new ScoreList("player name", 0)); // Add a new item to the high score list.
        highscore.Add(new ScoreList("player name", 0)); // Repeat.
        highscore.Add(new ScoreList("player name", 0)); // Repeat.
        highscore.Add(new ScoreList("player name", 0)); // Repeat.
        highscore.Add(new ScoreList("player name", 0)); // Repeat.
    }

    // This method takes the player input and places this in the high score table. Sorted in a descending order by score.
    public void AddEntry() {
        sceneControl.GetComponent<SceneController>().HighScoreScreenOn();   // Execute HighScoreScreenOn() from the SceneController script.
        tempName = inputField.text;                                         // The tempName variable stores the string entered into the input field by the player.
        playerScore = player.GetComponent<Player>().score;                  // Access the score variable from the Player script.
        highscore.Add(new ScoreList(tempName, playerScore));                // This highscore list adds a new item using the player input and score above.

        var mylist = highscore.OrderByDescending(x => x.score).ToList();    // A new list is created that is ordered by the score values.

        // A for loop that repeats five times.
        for (int i = 0; i < 5; i++) {                                       
            print(mylist[i].playerName + " " + mylist[i].score);            
            nameText[i].text = mylist[i].playerName;                        // An array element that stores the name from the newly ordered list.                    
            scoreText[i].text = mylist[i].score.ToString();                 // An array element that stores the score from the newly ordered list.
        }        
    }            

    void Update() {               
    }
}


