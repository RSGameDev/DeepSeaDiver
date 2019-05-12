using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This script sets the difficulty for the game. These are picked from the difficulty screen which occurs after pressing the Escape key.
// The NormalSettings() and HardSetting() are included in this script.
public class DifficultySettings : MonoBehaviour {           

    
    // Use this for initialization
    void Start () {
        NormalSettings();
	}
	
	// Update is called once per frame
	void Update () {		
	}        

    // This method sets the values for three variables once the normal difficulty option has been picked.
    public void NormalSettings() {
        EnemyAI.fieldOfViewAngle = 90f;             // The field of view for the enemy is set as 90 degrees.
        EnemyAI.timePerShot = 3f;                   // The enemy will be able to shoot every 3 seconds.
        Bolt.damage = 34;                           // The enemy's projectiles will have a damage value of 34.
    }

    // This method sets the values for three variables once the hard difficulty option has been picked.
    public void HardSetting() {
        EnemyAI.fieldOfViewAngle = 120f;            // The field of view for the enemy is set as 120 degrees.
        EnemyAI.timePerShot = 1.75f;                // The enemy will be able to shoot every 1.75 seconds.
        Bolt.damage = 50;                           // The enemy's projectiles will have a damage value of 50.
    }
}
