using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/****************************************************
 * Title: Patrol class (adapted from)
 * Author: Unity Learn Manual Documentation 
 * Availability: https://docs.unity3d.com/Manual/nav-AgentPatrol.html
 ****************************************************/
/****************************************************
* Title: Enemy Sight class (adapted from)
* For this script: From lines 99 to 140.
* Author: Official Unity Youtube channel  
* Availability: https://www.youtube.com/watch?v=mBGUY7EUxXQ
****************************************************/

// This script handles the enemies AI which involves movements, player detection and calling of the shoot function.
// The GotoNextPoint(), OnTriggerStay() and OnTriggerExit() are included in this script. 
public class EnemyAI : MonoBehaviour {

    GameObject player;                          // Reference to the player GameObject.
    NavMeshAgent agent;                         // Reference to the enemy NavMeshAgent.
    SphereCollider col;                         // Reference to the player detection SphereCollider.
    public Transform[] location;                // A reference for the enemy locations this game object can travel to.
    public Transform retreat;                   // A reference to the retreat location Transform.

    int health;                                 // A reference to health from the Enemy script.
    int locationPoint = 0;                      // The default value for the locations array.
    public static float fieldOfViewAngle = 90f; // The field of view the enemy will have.
    public static float timePerShot = 3f;       // The time in seconds in between shoots.
    float timer;                                // A general timer for the game.
    bool isRetreated;                           // Is the enemy currently retreating.
    bool retreatedOnce;                         // Has the enemy performed this retreat routine already.

    void Awake() {
        // Setting the references.
        agent = GetComponent<NavMeshAgent>();
        col = GetComponentInChildren<SphereCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        health = GetComponent<Enemy>().health;

    }

    // Use this for initialization
    void Start() {

        // Initialise values, this sets the enemy values at normal difficulty.
        fieldOfViewAngle = 90f;
        timePerShot = 3f;
        GotoNextPoint();                        // Execute GotoNextPoint().
    }

    // Update is called once per frame
    void Update() {

        timer += Time.deltaTime;                // The timer will keep track of the time since the game has started. This will aid the function for how often the enemy can shoot.

        health = GetComponent<Enemy>().health;  // Setting of a reference.
        

        // Choose the next destination point when the agent gets
        // close to the current one.     
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            GotoNextPoint();        
        }

        // Check if the game object attached to the navmeshagent is named "squid" AND the health is less than 50 AND is not currently retreating.
        //if (agent.name == "squid" && health < 50 && !isRetreated) {
        if (agent.name == "squid" && health < 50 && !isRetreated && !retreatedOnce) {
            isRetreated = true;                   // The enemy starts to retreat.
            retreatedOnce = true;                 // The enemy is acknowledged as having performed this retreat routine.  
            agent.speed = 4.5f;                   // The speed for the squid is set to 4.5f.
            agent.destination = retreat.position; // A new destination of the retreat position is set.  
        }
    }

    // Used to determine the next destination.
    void GotoNextPoint() {

        isRetreated = false;    // The enemy is not currently retreating.
        agent.speed = 3.5f;     // The speed of the enemy when patrolling to its next location.

        // Returns if no points have been set up
        if (location.Length == 0)
            return;

        // Check if the game object attached to the navmeshagent is named "jellyfish". 
        if (agent.name == "jellyfish") {
            agent.destination = location[Random.Range(0,location.Length)].position; // Set the destination for the navmeshagent to the position of a randomly choosen transform within the location array. 
        } else {
            // Set the agent to go to the currently selected destination.
            agent.destination = location[locationPoint].position;

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            locationPoint = (locationPoint + 1) % location.Length;
        }        
    }

    /**********************************************************************************
    * Adapted code using the Enemy Sight class from Unity's official Youtube channel
    ***********************************************************************************/

    void OnTriggerStay(Collider other) {

        // Check if the other collider's gameObject has the player reference attached to it.
        if (other.gameObject == player) {
            // Check if the gameObject is named "clownfish".
            if (gameObject.name == "clownfish") {
                
                transform.LookAt(player.transform.position);            // The game object will face the players position.
                agent.SetDestination(player.transform.position);        // The navmeshagent will set the players position as its new destination.
            }

            // Setting up the references
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // Check if the gameObject is named "squid" OR "jellyfish" AND is in the field of view for the enemy AND is not retreating.
            if ((gameObject.name == "squid" || gameObject.name ==  "jellyfish") && (angle < fieldOfViewAngle * 0.5f && !isRetreated)) {    
                RaycastHit hit;

                // Check if the raycast hits anything within its shooting range.
                if (Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius)) {

                    // Check if the RaycastHit collides with a gameObject that has the player reference attached to it.
                    if (hit.collider.gameObject == player) {
                        transform.LookAt(player.transform.position);     // The game object will face the players position
                        agent.SetDestination(player.transform.position); // The navmeshagent will set the players position as its new destination.
                        agent.speed = 3;                                 // The speed for the enemy will reduce to 3f.

                        // Check if the timer is greater or equal to the time assigned enabling the enemy to shoot.
                        if( timer >= timePerShot) {
                            timer = 0f;                                  // Set the timer to zero so the enemy is able to shoot again only when the above condition has been met.
                            GetComponent<Enemy>().EnemyFire();           // The enemy shoots. Using EnemyFire() found in the Enemy script.

                        }                        
                    }
                }
            }     
        }
    }

    /*End of adapted code**************************************************************/

    // When a collider leaves the player detection collider.
    void OnTriggerExit(Collider other) {

        // Check if the other collider is the player gameObject AND is not retreating.
        if (other.gameObject == player && !isRetreated) {
            GotoNextPoint();    // Execute GotoNextPoint(). The enemy will navigate to its next destination.
        }
    }      
}
