using UnityEngine;
using System.Collections;

public enum PickupType {Coin,Crate,Oxygen}; // The pickup types to be used for the script.

// This script handles the behaviours for the pickup spawner object for the game.
// The SpawnNewPickup() and PickUpCollected() are included in this script.
public class PickupSpawner : MonoBehaviour
{
    public PickupType spawnerPickupType;    // Reference to the spawnerPickupType enum.
    public float timeTillRespawn = 20;      // The time that pickups are spawn in.
    float timePassed;                       // The time that has passed since a pickup has been collected. 
    bool canSpawn;                          // Is a pickup able to spawn.

    public GameObject oxygenPrefab;         // Reference to the oxygen prefab GameObject.
    public GameObject coinPrefab;           // Reference to the coin prefab GameObject.
    public GameObject cratePrefab;          // Reference to the crate prefab GameObject.


    // Use this for initialization
    void Start()
    {
        timePassed = 0;                     // Setting a reference
        SpawnNewPickup();                   // Execute SpawnNewPickup().
        canSpawn = false;                   // Setting a reference
    }

    // Update is called once per frame
    void Update()
    {
        // Check that the time passed is less than the time till a pickup respawns.
        if(timePassed<timeTillRespawn)
        {
            timePassed += Time.deltaTime;   // The time passed will keep track of the time passed in game start. To help indicate when the correct time for a new pickup should spawn.
        }
        else
        {
            // Check if a new pickup can spawn.
            if (canSpawn)
                SpawnNewPickup();           // Execute SpawnNewPickup().
        }
    }

    // This method spawns new pickup objects.
    void SpawnNewPickup()
    {
        GameObject temp = null;             // The initial value for the temp GameObject.
        switch(spawnerPickupType)           // A switch statment that uses the spawnerPickupType enum.
        {
            // Case using the Coin enumerator.
            case PickupType.Coin:           
                // Initialise for temp with the coin prefab GameObject.
                temp = Instantiate(coinPrefab, transform.position, Quaternion.identity) as GameObject;
                break;
            // Case using the Crate enumerator.
            case PickupType.Crate:
                // Initialise for temp with the crate prefab GameObject.
                temp = Instantiate(cratePrefab, transform.position, Quaternion.identity) as GameObject;
                break;
            // Case using the Oxygen enumerator.
            case PickupType.Oxygen:
                // Initialise for temp with the oxygen prefab GameObject.
                temp = Instantiate(oxygenPrefab, transform.position, Quaternion.identity) as GameObject;
                break;            
        }
        temp.GetComponent<Pickup>().spawner = this.gameObject;  // Setting a reference
        canSpawn = false;                   // The pickup spawner is set to not being able to spawn.
    }

    // This method occurs when a pickup has been collected.
    public void PickUpCollected()
    {
        canSpawn = true;                    // The pickup spawner is allowed to spawn again.
        timePassed = 0;                     // Set this variable to zero so the check for spawning a new pickup can start over.
    }
}
