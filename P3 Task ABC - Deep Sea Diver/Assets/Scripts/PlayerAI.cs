using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// This script controls the player for the Atttact demo mode.
// The PlayerAIShoot() is included in this script.
public class PlayerAI : MonoBehaviour {

    NavMeshAgent agent;                 // Reference to the player NavMeshAgent.
    int shootAmount;                    // This will dictate how many times the player can shoot.
    int shotCount;                      // This is how many times the player has currently shot.
    float timer;                        // A timer from when the demo has started.
    public bool isEnemyInRange;         // This will change depending on if an enemy is in range or not.
    public bool isShootingDisabled;     // Has the player shot the number of projectiles that is required.

        
    // Use this for initialization
    void Start() {
        
        agent = GetComponent<NavMeshAgent>();           // Set up a reference.
        shootAmount = Random.Range(1, 3);               // Initialise shootAmount by giving it a random number. 
                                                        // This will be how many harpoons the player will shoot later on if required to.  
    }

    // Update is called once per frame
    void Update() {

        timer += Time.deltaTime;                        // The timer will keep track of the time since the game has started.
                                                        // This will aid the function for how often the player can shoot.
    }

    // This method occurs when a collider enters the players range detection collider.
    void OnTriggerStay(Collider other) {

        // Check if the other collider has the tag "enemymesh" AND the player is still able to shoot projectiles.
        if (other.CompareTag ("enemymesh") && !isShootingDisabled) {            
            isEnemyInRange = true;                      // An enemy is now in range
            transform.LookAt(other.transform.position); // The player will look at the enemy's position.            
            PlayerAIShoot();                            // Execute PlayerAIShoot(). This will start the method that allows the player to shoot.
        }
    }               

    // This method controls the shooting capabilties for the player AI.
    void PlayerAIShoot() {

        // A switch statement using the random shoot amount as a way to choose a case.
        switch (shootAmount) {
            case 1:
                // Check if the player has shot less than once.
                if (shotCount < 1) {                    
                    agent.speed = 1f;                       // The player reduces its speed to 1.
                    GetComponent<Player>().PlayerFire();    // The player shoots. Using PlayerFire() found in the Player script.
                    shotCount++;                            // The amount the player has shot increments by 1.
                    // Check if the player has shot an amount equal to 1.
                    if (shotCount == 1) {
                        isShootingDisabled = true;          // The player cannot shoot anymore. Temporarily, until the player has retreated and is out of range of the hostile.
                        shotCount = 0;                      // Reset the amount the player has fired.
                    }
                }      
            break;
            case 2:
                // Check if the time in the demo has past 1 second and the player has shot less than twice.
                if (timer >= 1f && shotCount < 2) {
                    timer = 0;                              // Set the timer to 0 so the remaining harpoons are able to fire.
                    agent.speed = 1f;                       // The player reduces its speed to 1.
                    GetComponent<Player>().PlayerFire();    // The player shoots. Using PlayerFire() found in the Player script.
                    shotCount++;                            // The amount the player has shot increments by 1.
                    // Check if the player has shot an amount equal to 2.
                    if (shotCount == 2) {
                        isShootingDisabled = true;          // The player cannot shoot anymore. Temporarily, until the player has retreated and is out of range of the hostile.
                        shotCount = 0;                      // Reset the amount the player has fired.
                    }
                }
            break;
            case 3:
                if (timer >= 1f && shotCount < 3) {
                    timer = 0;                              // Set the timer to 0 so the remaining harpoons are able to fire.
                    agent.speed = 1f;                       // The player reduces its speed to 1.
                    GetComponent<Player>().PlayerFire();    // The player shoots. Using PlayerFire() found in the Player script.
                    shotCount++;                            // The amount the player has shot increments by 1.
                    // Check if the player has shot an amount equal to 3.
                    if (shotCount == 3) {
                        isShootingDisabled = true;          // The player cannot shoot anymore. Temporarily, until the player has retreated and is out of range of the hostile.
                        shotCount = 0;                      // Reset the amount the player has fired.
                    }
                }
            break;
        }
    }        

    // This method takes place when a collider exits the player's detection range collider.
    void OnTriggerExit(Collider other) {

        // Check if the other collider has the tag "enemymesh".
        if (other.CompareTag("enemymesh")) {
            isEnemyInRange = false;                         // The enemy is now not in range.
        }
    }
}
