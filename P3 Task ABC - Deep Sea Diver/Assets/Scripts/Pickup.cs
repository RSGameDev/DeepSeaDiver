using UnityEngine;
using System.Collections;

// This script handles the behaviour for all pickups.
// The Collected() is included in this script.
public class Pickup : MonoBehaviour
{
    GameObject player;              // Reference to the player GameObject.
    public GameObject spawner;      // Reference to the spawner GameObject.
    

    void Awake() {
        // Setting a reference
        player = GameObject.Find("First Person Controller");
    }


    void Start() {
        // Ignore any scene range colliders so the pickup does not destroy itself involuntarily.
        Physics.IgnoreCollision(player.GetComponentInChildren<SphereCollider>(), GetComponent<BoxCollider>());        
    }

    // Update is called once per frame
    void Update() {
    }

    // This method occurs when a pickup has been collected
    public void Collected()
    {
        spawner.GetComponent<PickupSpawner>().PickUpCollected();    // Execute PickUpCollected() from the PickupSpawner Script.  
        Destroy(this.gameObject);                                   // Destroy this bullet game object.
    }
}
