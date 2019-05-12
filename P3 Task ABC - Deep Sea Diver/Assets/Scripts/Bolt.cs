using UnityEngine;
using System.Collections;

/****************************************************
 * Title: ExampleClass  (Based from)
 * For this script: From lines 90 to 107.
 * Author: Unity Scripting Documentation 
 * Availability: https://docs.unity3d.com/ScriptReference/GameObject.FindGameObjectsWithTag.html
 ****************************************************/

// This script handles the behaviour for the projectiles for the game.
// The FindClosestEnemy() for the heek seeking ability is included in this script.
public class Bolt : MonoBehaviour
{
    GameObject player;                  // Reference to the player GameObject.
    GameObject[] enemies;               // A reference for an array of enemy game objects.
    GameObject closest = null;          // Initialise the closest GameObject to the value of null. A reference for the closest heat seek enemy target.
    public AudioClip harpoonSound;      // Reference to the audio clip for when a harpoon is fired.
    public AudioClip projectileSound;   // Reference to the audio clip for when an enemy shoots a projectile.
    public AudioClip explosiveHitSound; // Reference to the audio clip for when an explosive projectile hits.

    ParticleSystem normalBubbles;       // Reference to the bubble trail ParticleSystem that appears from shooting a projectile.
    ParticleSystem harpoonExplosive;    // Reference to the explosion ParticleSystem that appears when a projectile hits an enemy. When the explosive pickup is activated.

    public static int damage = 34;      // The damage value for a projectile.
    float speed = 15;                   // The speed that a projectile will move at.
    float lifeCounter;                  // The time that the projectile will exist before being destroyed.

    bool isHeatSeekOn;                  // Has the heat seeking projectile pickup been collected. 
    bool isExplosiveOn;                 // Has the explosive projectile pickup been collected.


    void Awake() {
        // Set up a reference.
        player = GameObject.Find("First Person Controller");        

    }

    // Use this for initialization
    void Start() {
        
        // Check if the string "bolt" appears in the gameObjects name.
        if (gameObject.name.Contains("bolt")) {
            // Setting up references.
            normalBubbles = transform.Find("bubbles").GetComponent<ParticleSystem>();
            harpoonExplosive = transform.Find("explosion").GetComponent<ParticleSystem>();
            AudioManager.instance.PlayerPlayClip(harpoonSound);     // The shoot harpoon sound plays
        }

        // Check if the tag of this game object is "enemy bullet". 
        if (tag == "enemy bullet") {
            AudioManager.instance.PlayerPlayClip(projectileSound);  // The enemy projectile shoot sound plays.
            transform.LookAt(player.transform.position);            // The projectile will be aimed at the players position.
        }       
        
        // Setting up references.
        isHeatSeekOn = player.GetComponent<Player>().isHeatSeek;
        isExplosiveOn = player.GetComponent<Player>().isExplosive;

        // Check if the player has the explosive pickup activated.
        if (isExplosiveOn) {            
            var tempMain = normalBubbles.main;                      // Reference a local variable.
            tempMain.startColor = new Color(240, 174, 0, 255);      // Set the start colour for the projectile bubble trail to orange.
        }

        GetComponent<Rigidbody>().velocity = transform.forward * speed; // Allow the bullet to travel in a forward trajectory.      

    }

    // Update is called once per frame
    void Update()
    {      
        lifeCounter += Time.deltaTime;      // Count how long bolt is alive.
        
        // Check if the projectile has existed for more than 20 seconds.
        if(lifeCounter>20)
        {
            Destroy(this.gameObject);       // Destroy it when it is over 20 seconds old.
        }

        // Check if the player has the heat seek projectile pickup activated.
        if (isHeatSeekOn) {
            FindClosestEnemy();                                             // Execute FindClosestEnemy(). This will perform the calculations for the heat seeking projectile to work.                                                 
            Transform target = closest.transform;                           // Initialise target to the transform of the closest game object calculated in FindClosestEnemy().
            GetComponent<Transform>().LookAt(target);                       // The projectile will look at the target. The projectile will not lose sight of this target due to this occuring within Update(). 
            GetComponent<Rigidbody>().velocity = transform.forward * speed; // Allow the bullet to travel in a forward trajectory.      
        }        

    }

    /***********************************************************************************************************************************
    * Code based of the FindClosestEnemy() from Unity's scripting documentation. Found on the GameObject.FindGameObjectsWithTag webpage.
    ************************************************************************************************************************************/

    // This method calculates the closest enemy for the heat seeking projectile pickup.
    public GameObject FindClosestEnemy() {
        enemies = GameObject.FindGameObjectsWithTag("enemy");               // Tag the enemies in an array with the tag "enemy".
        float distance = Mathf.Infinity;                                    // The distance default is initialised.
        Vector3 position = transform.position;                              // Initialise for the projectile the position variable for this method.
        // The foreach statement will in the enemies array for each enemy.        
        foreach (GameObject enemy in enemies) {
            Vector3 diff = enemy.transform.position - position;             // Calculate the difference between the enemy position and the position of the projectile.
            float curDistance = diff.sqrMagnitude;                          // Initialise the current distance using the difference calculated just above and using sqrMagnitude.
            // Check if the current distance is less than the baseline distance from the start of this method.
            if (curDistance < distance) {
                closest = enemy;                                            // Make closest game object the enemy from the occuring foreach statement.
                distance = curDistance;                                     // The distance for this method will now take the value of current distance. This will facilitate if an enemy is closer to the player
                                                                            // than the closest game object assigned at present when the foreach statement repeats itself..
            }
        }
        return closest;                                                     // The closest game object is returned. This is the enemy game object calculated to be the nearest to the player. So the heat seek projectile
                                                                            // will guide its way to this enemy game object.
    }

    /*End of code based*****************************************************************************************************************/

    // This method occurs when the projectile collides with another collider.
    void OnTriggerEnter(Collider other) {

        // Check if the other collider has the tag "enemy range". This refers to the enemies' detect player range collider.
        if (other.CompareTag("enemy range")) {
            return;                                                         // The projectile will carry on as usual.                                                                
        }

        // Check if the other collider has the tag "terrain".
        if (other.CompareTag("terrain")) {
            Destroy(gameObject);                                            // Destroy the projectile game object.
        }

        // Check if the other collider has the tag "enemymesh" AND this game object has the tag "player bullet" AND the player does not have the explosive pickup activated at present.
        // This refers to the whether the projectile is shot by the player and hits an enemy. While not being in use of the explosive pickup.
        if ((other.CompareTag("enemymesh")) && tag == "player bullet" && !isExplosiveOn) {
            other.GetComponentInParent<Enemy>().DamageTaken(damage);        // Execute DamageTaken() from the Enemy Script. This will perform the effects of the projectile hitting the enemy.
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);      // The projectile will stop its momentum.
            Destroy(gameObject);                                            // Destroy the projectile game object.    
        }

        // Check if the other collider has the tag "enemymesh" AND this game object has the tag "player bullet" AND the player has the explosive pickup activated.
        // This refers to the whether the projectile is shot by the player and hits an enemy. While the explosive projectile ability is active.
        if ((other.CompareTag("enemymesh")) && tag == "player bullet" && isExplosiveOn) {
            int explosiveDamage = 100;                                       // The damage of the projectile while the explosive ability is active.
            other.GetComponentInParent<Enemy>().DamageTaken(explosiveDamage);// Execute DamageTaken() from the Enemy Script. This will perform the effects of the projectile hitting the enemy. With the new damage value.
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);       // The projectile will stop its momentum.
            AudioManager.instance.PlayerPlayClip(explosiveHitSound);         // The explosive hit sound plays.
            harpoonExplosive.Play();                                         // Play the explosive effect from when the projectile hits the enemy.    
            Destroy(gameObject, 1.5f);                                       // Destroy the projectile game object after 1.5 seconds. To allow for the particle system above to run through its animation.
            return;
        }

        // Check if the other collider has the tag "Player" AND this game object has the tag "enemy bullet".
        // This refers to the enemy shooting the player.
        if ((other.CompareTag("Player")) && tag == "enemy bullet") {            
            other.GetComponent<Player>().DamageTaken(damage);               // Execute DamageTaken() from the Player Script. This will perform the effects of the projectile hitting the enemy.
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);      // The projectile will stop its momentum.
            Destroy(gameObject);                                            // Destroy the projectile game object.   
        }
    }    
}