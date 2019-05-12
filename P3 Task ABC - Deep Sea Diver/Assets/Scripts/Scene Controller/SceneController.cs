using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script manages the screens that appear on the screen. The attract demo is started within this script. The high score table is displayed here also.
// The DisplayDifficultyScreenOn(), InputEntry(), HighScoreScreenOn(), LoadLevel(), ExitGame() are included in this script.
public class SceneController : MonoBehaviour {

    public GameObject titleScreen;              // Reference to the title screen GameObject.
    public GameObject briefingScreen;           // Reference to the briefing screen GameObject.
    public GameObject userInterface;            // Reference to the user interface GameObject.
    public GameObject difficultyScreen;         // Reference to the difficulty screen GameObject.
    public GameObject completedScreen;          // Reference to the completed screen GameObject.
    public GameObject highScoreScreen;          // Reference to the high score screen GameObject.
    public GameObject highscoreText;            // Reference to the high score text GameObject.
    public GameObject highscoreText1;           // Reference to the high score text 1 GameObject.
    public GameObject panelInput;               // Reference to the panel for the player input GameObject.
    public GameObject panelHighscore;           // Reference to the panel for the high score GameObject.
    public AudioClip MainGame;                  // Reference to the audio clip which will play the main game soundtrack.

    GameObject player;                          // Reference to the player GameObject.
    GameObject cameraView;                      // Reference to the camera GameObject.

    float timer;                                // This timer is used for how long the attract demo has been running for.
    float attractTimer;                         // This timer is used to determine if the attract demo can start.
    public bool isAttractHasRun;                // Has the attract demo occured already.
    static bool isDemoFinished;                 // Has the attract demo finished running. From either player input or the demo running for 30 seconds already.
    public bool isPlayerStartedGame;            // Has the player started the game manually from clicking the "Play Game" button.
    bool isDifficultyScreenOn;                  // Has the difficulty screen been made to appear. This is carried out by pressing the escape key.
    
    private void Awake() {
        titleScreen.SetActive(true);            // The title screen game object will become activated. This was deactivated prior due to this screen obscuring my view during development of the game.
        briefingScreen.SetActive(true);         // The briefing screen game object will become activated. For the same reasons as above, obscuring view during development.
        userInterface.SetActive(true);          // The UI screen game object will become activated.
        difficultyScreen.SetActive(true);       // The difficulty screen game object will become activated. 
        highScoreScreen.SetActive(true);        // The high score screen game object will become activated. 

        // Setting up references.
        player = GameObject.Find("First Person Controller");
        cameraView = GameObject.Find("Main Camera");
    }

    // Use this for initialization
    void Start() {     
        UnityEngine.Cursor.visible = true;                          // The cursor for the mouse will become visible. So the player can interact with the Start Screen.
        titleScreen.GetComponent<Canvas>().enabled = true;          // The title screen canvas will become enabled. For the start of the game this is the only screen the player needs to see at the beginning.
        briefingScreen.GetComponent<Canvas>().enabled = false;      // The briefing screen canvas will be disabled. This screen is not required yet.
        userInterface.GetComponent<Canvas>().enabled = false;       // The UI screen canvas will be disabled. This screen is not required yet.    
        difficultyScreen.GetComponent<Canvas>().enabled = false;    // The difficulty screen canvas will be disabled. This screen is not required yet.
        highScoreScreen.GetComponent<Canvas>().enabled = false;     // The high score canvas will be disabled. This screen is not required yet.
    }

    // Update is called once per frame
    void Update() {

        // Check if the attract demo is not currently in progress AND the player has not started the game manually AND the demo has not played through once already.
        if (!isAttractHasRun && !isPlayerStartedGame && !isDemoFinished) {
            attractTimer += Time.deltaTime;                         // The timer that determines if the attract demo should commence begins recording the time. In another condition when this reaches 10 seconds the attract demo will begin.
        }

        // Check if the attractTimer is more than or equal to 10 seconds. The attract demo commences here.
        if (attractTimer >= 10f) {
            player.GetComponent<Player>().enabled = true;
            player.GetComponent<Player>().oxygenLevel = 100;        // Reset the player's oxygen to 100 for the demo.
            attractTimer = 0;                                       // The attractTimer is set to 0 to reset this. Otherwise this condition would repeat frame after frame.
            isAttractHasRun = true;                                 // The attract demo is recorded as having started here. 
            titleScreen.GetComponent<Canvas>().enabled = false;     // The title screen canvas will be disabled. The attract demo will now take place with the player AI taking control in the demo.
        }

        // Check if the attract demo is in progress. This body will determine when the demo will stop running due to player input or the demo time expiring.
        if (isAttractHasRun) {

            timer += Time.deltaTime;                                // The time since the demo has started.

            // Check if any input has been detected from keyboard or mouse OR the time the demo has run for is greater than or equal to 30 seconds.
            if (Input.anyKeyDown || timer >= 30f) {
                isDemoFinished = true;                              // The attract demo is set to having finished here.
                LoadLevel();                                        // Execute LoadLevel(). The Start Screen will load so the player can begin playing the game.
            }
        }        

        // Check if the Return key has been pressed.
        if (Input.GetKeyDown(KeyCode.Return)) {
            BriefingScreenOff();                                    // Execute BriefingScreenOff(). The briefing screen canvas will be disabled. The game is now ready to be played.
            
        }

        // Check if the Escape key has been pressed.
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Check if the difficulty screen is not in use.
            if (!isDifficultyScreenOn) {                                
                DisplayDifficultyScreenOn();                        // Execute DisplayDifficultyScreenOn(). The difficulty screen canvas will become visible.
            } else {
                DisplayDifficultyScreenOff();                       // Execute DisplayDifficultyScreenOff(). The difficulty screen canvas will be disabled. The screen will be removed.
            }
        }        
        
        // Check if the End key has been pressed. The game will reset to the Start Menu.
        if (Input.GetKeyDown(KeyCode.End)) {
            LoadLevel();                                            // Execute LoadLevel(). The Start Screen will reload for the player to start another game or exit the game.         
        }
    }

    // This method will remove the title screen from the display.
    public void TitleScreenOff() {
        isPlayerStartedGame = true;                                 // This sets the variable to true so that the attract demo will not be able to start. As this would interrupt the player being able to play the game.
        UnityEngine.Cursor.visible = false;                         // The cursor for the mouse will become invisible. As this is no longer necessary for the player to see.
        titleScreen.GetComponent<Canvas>().enabled = false;         // The title screen canvas will be disabled. The briefing screen will need to be displayed next.
        briefingScreen.GetComponent<Canvas>().enabled = true;       // The briefing screen canvas will become visible. This will give the player some information prior to starting the game.
    }

    // This method will exit the game. This occurs through clicking the "Exit Game" button in the Start Menu.
    public void ExitGame() {
        Application.Quit();                                         // The game will close down. 
    }

    // This method will remove the briefing screen from the display.
    void BriefingScreenOff() {
        player.GetComponent<Player>().enabled = true;
        AudioManager.instance.MusicClip(MainGame);                  // The main game soundtrack will play.
        player.GetComponent<Player>().oxygenLevel = 100;            // Reset the player's oxygen to 100 for the game.
        briefingScreen.GetComponent<Canvas>().enabled = false;      // The briefing screen canvas will be disabled. After this the game will be visible where the player can start playing the game.
        userInterface.GetComponent<Canvas>().enabled = true;        // The UI screen canvas will be enabled. Providing the player with essential game information like health, score, ammo.
    }

    // This method will display the difficulty screen where the player can choose from a normal or hard difficulty setting.
    void DisplayDifficultyScreenOn() {
        isDifficultyScreenOn = true;                                // This will set the difficulty screen as being currently active.
        Time.timeScale = 0f;                                        // The game is set to being in a paused state so in the background nothing is moving.    
        difficultyScreen.GetComponent<Canvas>().enabled = true;     // The difficulty screen canvas will become visible. This will offer the player a choice of picking a normal or hard difficulty setting.
        userInterface.SetActive(false);                             // The UI screen game object will become deactivated. To avoid clutter on display.
        UnityEngine.Cursor.visible = true;                          // The cursor for the mouse will become visible. So the player can interact with this screen.
        cameraView.GetComponent<MouseLook>().enabled = false;       // This script is disabled so when the cursor is moving the background will not be seen to be moving along with the cursor.
        player.GetComponent<Player>().enabled = false;              // This script is disabled so the player will not be seen to be moving as well along with the cursor when this is being moved around the screen.
    }

    // This method will remove the difficulty screen from the display.
    void DisplayDifficultyScreenOff() {
        isDifficultyScreenOn = false;                               // This will set the difficulty screen as being inactive.
        Time.timeScale = 1f;                                        // The game is set to run at normal speed.
        difficultyScreen.GetComponent<Canvas>().enabled = false;    // The difficulty screen canvas will be disabled. So the game can then resume.
        userInterface.SetActive(true);                              // The UI screen game object will become activated. Bringing back the essential in game information for the player like health, score, ammo.
        UnityEngine.Cursor.visible = false;                         // The cursor for the mouse will become invisible. As this is no longer required.
        cameraView.GetComponent<MouseLook>().enabled = true;        // This script is enabled so this can function as before. So the player is able to look around the screen.
        player.GetComponent<Player>().enabled = true;               // This script is enabled so this can function as before. So the player can move around as normal.
    }  
    

    // This method displays the interface for the player to input their name after they have died or completed the game.
    public void InputEntry() {
        highScoreScreen.GetComponent<Canvas>().enabled = true;      // The high score screen canvas will become visible. The player will input a string here which will then be displayed on the high score table.
        panelInput.SetActive(true);                                 // The panel where the player input takes place will become activated and show up for the player on the screen.    
        cameraView.GetComponent<MouseLook>().enabled = false;       // This script is disabled so when the cursor is moving the background will not be seen to be moving along with the cursor.
        player.GetComponent<Player>().enabled = false;              // This script is disabled so the player will not be seen to be moving as well along with the cursor when this is being moved around the screen.
        UnityEngine.Cursor.visible = true;                          // The cursor for the mouse will become visible. So the player can interact with this screen.
    }

    // This method displays the high score table for the player. This appears after the player has entered a string into the input field.
    public void HighScoreScreenOn() {
        completedScreen.SetActive(false);                           // The completed screen message is removed from the display.
        highscoreText.SetActive(true);                              // The text for this high score screen will be activated so will now be visible to the player.
        highscoreText1.SetActive(true);                             // A second text for this high score screen will be activated so will now be visible to the player.
        panelInput.SetActive(false);                                // The panel where the player inputs a string into will now be deactivated.   
        panelHighscore.SetActive(true);                             // The panel for the high score table will be activated and so the high score table will be visible to the player.
    }

    // This method will load a scene in the game. The Start Menu will be loaded in this case.
    void LoadLevel() {
        Time.timeScale = 1f;                                        // The game is set to run at normal speed once more.
        SceneManager.LoadScene(0);                                  // The game will load the scene with a build index of 0. This will be for the Start Menu.
    }
}