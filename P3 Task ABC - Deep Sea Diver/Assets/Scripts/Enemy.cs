using UnityEngine;
using System.Collections;
using UnityEngine.AI;

// This script controls the behaviours for the enemies in game.
// The DamageTaken() and EnemyFire() are included in this script.
public class Enemy : MonoBehaviour
{
    Player playerscript;                    // Reference to the player script.
    public GameObject player;               // Reference to the player GameObject.
    public AudioClip damageSound;           // Reference to the audio clip that plays when the enemy receives damaged.

    public int health = 100;                // The health for the enemy.
    
    public GameObject bulletPrefab;         // Reference to the bullet prefab GameObject.
    public GameObject bulletSpawn = null;   // Reference to the bullet spawn GameObject initialised to null.

    void Awake() {
        // Setting a reference
        playerscript = player.GetComponent<Player>();
    }

    // Use this for initialization
    void Start()  {        
    }

    // Update is called once per frame
    void Update() {        
    }

    // This method deals with the damage taken when hit by a projectile.
    public void DamageTaken(int damage) {

        health -= damage;                                       // The health of then enemy deducted by the damage value of the projectile.
        AudioManager.instance.EnemyPlayClip(damageSound);       // The enemy receives damage sound is played.

        // Check if the health is less than 50 AND this game object is named "squid".
        if (health < 50 && gameObject.name == "squid") {
            GetComponentInChildren<ParticleSystem>().Play();    // Play the ink cloud particle system.
        }

        // Check if the health is equal to or less than zero.
        if (health <= 0) {                      
            playerscript.score += 100;                          // Add the score of the enemy, 100. To the score variable in the player script.
            Destroy(gameObject);                                // Destroy this game object.
        }
    }

    // This method handles the operations for when the enemy shoots a projectile.
    public void EnemyFire() {

        // A local game object variable is created that is referenced to an instantiated bullet prefab. The position and rotation are set to the bulletSpawn game object.
        var bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.gameObject.tag = "enemy bullet";                 // Assign the bullet the tag "enemy bullet".
    }      
}
