using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;

// This script handles the functionality for the player in play mode and during the attract demo mode.
// The ClosestPickUp(), PlayerFire(), DamageTaken() can be found within the script.
public class Player : MonoBehaviour {
        
    public GameObject sceneControl;     // Reference to the scene controller GameObject.
    public GameObject userInterface;    // Reference to the user interface GameObject.
    public GameObject completedScreen;  // Reference to the completed screen GameObject.
    public GameObject gameOverScreen;   // Reference to the game over screen GameObject.
    public Text heatSeekOn;             // Reference to the text that displays when the heat seeker pickup has been collected.
    public Text rapidFireOn;            // Reference to the text that displays when the rapid fire pickup has been collected.
    public Text threeWayOn;             // Reference to the text that displays when the three way shot pickup has been collected.
    public Text explosiveOn;            // Reference to the text that displays when the explosive projectile pickup has been collected.    
    public Text shieldOn;               // Reference to the text that displays when the shield pickup has been collected.
    public Text heatSeekOff;            // Reference to the text that displays when the heat seeker pickup deactivates.
    public Text rapidFireOff;           // Reference to the text that displays when the rapid fire pickup deactivates.
    public Text threeWayOff;            // Reference to the text that displays when the three way shot pickup deactivates.
    public Text explosiveOff;           // Reference to the text that displays when the explosive projectile pickup deactivates.
    public Text shieldOff;              // Reference to the text that displays when the shield pickup deactivates.
    public GameObject boltPrefab;       // Reference to the projectile prefab GameObject.
    public GameObject boltSpawn;        // Reference to the projectile spawn point GameObject.
    public GameObject spine;            // Reference to an alternative projectile spawn point GameObject.
    public GameObject boltSpawnL;       // Reference to the spawn point that allows a projectile to shoot at an angle to the left (three way shoot pickup).
    public GameObject boltSpawnR;       // Reference to the spawn point that allows a projectile to shoot at an angle to the right (three way shoot pickup).   
    public GameObject animationModel;   // Reference to the animations the GameObject can perform. 
    public ParticleSystem shieldPS;     // Reference to the shield ParticleSystem.
    public AudioClip hurtSound;         // Reference to the audio clip which will play when the player is shot.
    public AudioClip deathSound;        // Reference to the audio clip which will play when the player dies.

    public int score = 0;               // The score for the player starts at zero.
    int ammoLevel;                      // This will contain the amount of ammunition the player is in possession of.
    int remainingCoins = 6;             // The amount of coins the player has to collect in order to complete the level.
    float currentHealth;                // The health of the player.
    float timeHS;                       // The time that the heat seeker pickup has been active for.
    float timeRF;                       // The time that the rapid fire pickup has been active for.
    float time3W;                       // The time that the three way shot pickup has been active for.
    float timeEx;                       // The time that the explosive projectile pickup has been active for.
    float timeShield;                   // The time that the shield pickup has been active for.
    public float oxygenLevel;                  // The amount of oxygen the player has remaining.
    float oxygenMax = 100;              // The maximum amount of oxygen the player can have.
    bool isDead;                        // Is the player dead.

    #region UI text
    public Text coinsText;           // Reference to the coins value Text.
    public Text scoreText;           // Reference to the score value Text.
    public Text oxygenText;          // Reference to the oxygen value Text.
    public Text ammoText;            // Reference to the ammo value Text.
    public Text healthText;          // Reference to the health value Text.
    #endregion

    #region Attract mode
    GameObject closestPowerUp;          // Reference to the closest pick up to the player GameObject.
    public Transform[] attractPoints;   // A reference to the start location array for the attract demo.
    Vector3 attractStartPos;            // A reference to the start location that has been choosen at random.
    NavMeshAgent agent;                 // Reference to the player NavMeshAgent.
    float shortestDistance;             // The shortest distance from the player and pickup. Used in ClosestPickUp(). 
    bool closestOnce;                   // When the ClosestPickUp() has performed, this will change to true. Implemented for script functionality purposes.
    bool isEnemyThreat;                 // Is the enemy in range of firing. This is a reference for isEnemyInRange from the PlayerAI script.
    bool isAttractMode;                 // Is the attract mode in progress. This is a reference for isAttractHasRun from the SceneController script.
    bool isAttractOn;                   // Specific for this script. Is the attract mode in motion.
    bool isRunning;                     // A flag that occurs when the ClosestPickUp() has commenced, this allows for the player to animate as well through a condition check in Update().
    bool isRetreat;                     // Is the player retreating from the enemy. This is a reference for isShootingDisabled from the PlayerAI script.
    bool isStartedGame;                 // Has the player started the game manually. This is a reference for isPlayerStartedGame from the SceneController script.
    bool isStartedFlag;                 // When the player clicks 'Play Game' from the Start Menu this will change to true. Allows for the pickups and items for the game to generate into the scene.
    #endregion

    #region scene pickups
    GameObject coinObj;                 // Reference to the coin object included in the attract demo.
    GameObject crateObj;                // Reference to the crate object included in the attract demo.
    GameObject oxygenObj;               // Reference to the oxygen object included in the attract demo.
    public GameObject coinObj1;         // Reference to a coin object included in the playable game.
    public GameObject coinObj2;         // Reference to a coin object included in the playable game.
    public GameObject coinObj3;         // Reference to a coin object included in the playable game.
    public GameObject coinObj4;         // Reference to a coin object included in the playable game.
    public GameObject coinObj5;         // Reference to a coin object included in the playable game.     
    public GameObject crateObj1;        // Reference to a crate object included in the playable game.    
    public GameObject oxygenObj1;       // Reference to a oxygen object included in the playable game.
    public AudioClip coinSound;         // Reference to the audio clip which will play when a coin is collected.
    public AudioClip powerUpSound;      // Reference to the audio clip which will play when any pickup is collected.
    #endregion

    #region Shooting
    public int shotType = 0;             
    float secsPerShot = 0.5f;           // How long in seconds before the next projectile can be fired when rapid fire is active.
    float secondsOfRapidFire = 10.0f;   // How long the rapid fire ability will last for, 10 seconds indicated here.
    float rapidFireStart;               //
    float threeWayStart;
    float secondsofThreeWay = 10.0f;    // How long the three way shot ability will last for, 10 seconds indicated here.     
    public bool isHeatSeek;             // Has the heat seeking projectile pickup been collected.
    public bool isRapidFire;            // Has the rapid fire pickup been collected.
    public bool isThreeWay;             // Has the three way shot pickup been collected.
    public bool isExplosive;            // Has the explosive projectile pickup been collected.
    public bool isShield;               // Has the shield pickup been collected.
    #endregion

    
    void Awake() {
        
        // Setting up references.
        shieldPS = GameObject.Find("ShieldPS").GetComponent<ParticleSystem>();
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;                                      // The NavMeshAgent is set to disabled initially. This is only needed if the attract demo is in progress.
        GetComponentInChildren<SphereCollider>().enabled = false;   // The SphereCollider (enemy detection) is set to disabled initially. This is only needed if the attract demo is in progress.      
    }

    // Use this for initialization
    void Start() {

        // Setting up references.
        GetComponent<Player>().enabled = false;
        isRetreat = GetComponent<PlayerAI>().isShootingDisabled;
        isEnemyThreat = GetComponent<PlayerAI>().isEnemyInRange;

        oxygenLevel = 100;              // Set the oxygen level for the player to 100.            
        ammoLevel = 50;                 // Set the ammunition for the player to 50.
        currentHealth = 100;            // Set the health of the player to 100.

        StartCoroutine(ReferenceObj()); // The items for the attract demo will be referenced using this routine.      
    }

    // This routine will reference the attract demo items for the scene.
    IEnumerator ReferenceObj() {
        yield return new WaitForSeconds(1f);                        // Wait for 1 second before continuing with the routine.
        // Setting up references.
        coinObj = GameObject.FindGameObjectWithTag("coin");  
        crateObj = GameObject.FindGameObjectWithTag("crate");
        oxygenObj = GameObject.FindGameObjectWithTag("oxygen"); 
    }

    // Update is called once per frame
    void Update() {

        // Setting up references. These will be updated every frame so they can have an immediate affect to the script when necessary.  
        isStartedGame = sceneControl.GetComponent<SceneController>().isPlayerStartedGame;
        isAttractMode = sceneControl.GetComponent<SceneController>().isAttractHasRun;
        isRetreat = GetComponent<PlayerAI>().isShootingDisabled;
        isEnemyThreat = GetComponent<PlayerAI>().isEnemyInRange;

        // Check has the player started the game AND the game has not started previously.
        if (isStartedGame && !isStartedFlag) {
            CreatePickUps();            // Execute CreatePickUps(). This will create the objects for the game and have them appear in the level.                             
        }

        // Check has the attract mode been triggered to true AND the attract demo is not currently running AND a flag for is the attract demo not currently running.
        if (isAttractMode && !isAttractOn && !isRunning) {            
            isAttractOn = true;                                     // The attract demo has started.
            agent.enabled = true;                                   // The player can now be controlled via scripting with the NavMeshAgent attached to it. 
            int ran = Random.Range(0, attractPoints.Length);        // Choose a random start point from an array containing different start locations.
            attractStartPos = attractPoints[ran].position;          // Store this position in attractStartPos. This is required to allow the player to retreat in the demo.
            transform.position = attractStartPos;                   // The player will be placed at the position chosen above.
            transform.rotation = attractPoints[ran].rotation;       // This will have the player face the correct direction.
            GetComponentInChildren<SphereCollider>().enabled = true;// The enemy range detection collider will be enabled so the PlayerAI script can function where needed.            
            ClosestPickUp();                                        // Execute ClosestPickUp(). This will calculate the closest pickup to the players current position. 
        }

        // Check if the ClosestPickUp() has run AND the player is not retreating.
        if (closestOnce && !isRetreat) {
            // If the player does not have a closest pickup to collect.
            if (closestPowerUp == null) {
                ClosestPickUp();                                    // Execute ClosestPickUp(). So the player can resume collecting pickups.
            }
        }

        // Check if the player is retreating.
        if (isRetreat) {
            agent.destination = attractStartPos;                    // The destination for the players NavMeshAgent is attractStartPos. This is the position the player originally spawned from at the start of the demo.
            agent.speed = 4.5f;                                     // The speed for the player will change to 4.5 to imply the urgency the player is in to escape from the threat.
            // Check if the enemy is out of the players range.
            if (!isEnemyThreat) {
                GetComponent<PlayerAI>().isShootingDisabled = false;// The player is now able to shoot again. As the player is not in the motion of retreating.                
                ClosestPickUp();                                    // Execute ClosestPickUp(). So the player can resume collecting pickups.
            }
        }

        // Set up a reference.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the raycast hit anything upto 1000 distance away.
        if (Physics.Raycast(ray, out hit, 1000)) {
            spine.transform.LookAt(hit.point);                      // The player's spine will face towards this point in world space.
        }
        
        // Check if the left mouse button is clicked AND the player has more than 0 ammunition.
        if (Input.GetMouseButtonDown(0) && ammoLevel > 0) {
            PlayerFire();                                           // Execute PlayerFire(). This performs the shooting mechanics for the player.
            ammoLevel--;                                            // Decrement the ammunition the player has by 1.
        }                
        
        // Check if the left mouse button is held down AND the player has rapid fire activated.
        if (Input.GetMouseButton(0) && isRapidFire) {
            // Check if the time since the pickup has been activated is equal to or more than seconds per shot.
            if (timeRF >= secsPerShot) {                            
                PlayerFire();                                       // Execute PlayerFire(). This performs the shooting mechanics for the player.
            }               
        }

        // Check if the heat seek pickup is activated.
        if (isHeatSeek) {
            timeHS += Time.deltaTime;                               // The time since this pickup has been activated.
            // Check if the time the pickup has been activate for is equal to or more than 10 seconds.
            if (timeHS >= 10f) {
                isHeatSeek = false;                                 // The heat seek ability becomes deactivated.
                StartCoroutine(HeatSeekPickUpOff());                // The text that tells the player this ability is deactivated will appear.
            }
        }

        // Check if the explosive projectile pickup is activated.
        if (isExplosive) {
            timeEx += Time.deltaTime;                               // The time since this pickup has been activated.
            // Check if the time the pickup has been activate for is equal to or more than 10 seconds.
            if (timeEx >= 10f) {
                isExplosive = false;                                // The explosive projectile ability becomes deactivated.
                StartCoroutine(ExplosivePickUpOff());               // The text that tells the player this ability is deactivated will appear.
            }
        }

        // Check if the rapid fire pickup is activated.
        if (isRapidFire) {            
            timeRF += Time.deltaTime;                               // The time since this pickup has been activated.
                // Check if the time the pickup has been activate for is equal to or more than the pickup expire time, which has been set at 10 seconds.
                if (timeRF >= secondsOfRapidFire) {                
                    isRapidFire = false;                            // The rapid fire ability becomes deactivated.
                    timeRF = 0.0f;                                  // Reset the time for the rapid fire pickup to zero in the event this needs to be used again.
                    StartCoroutine(RapidFirePickUpOff());           // The text that tells the player this ability is deactivated will appear.
            }
        }

        // Check if the left mouse button is clicked AND the player has the three way shot activated.
        if (Input.GetMouseButtonDown(0) && isThreeWay) {

            // Referencing the projectiles.
            var bulletL = Instantiate(boltPrefab, boltSpawn.transform.position, boltSpawnL.transform.rotation); // Clone the projectile prefab and set the spawn point and rotation for it. This is for the projectile that shoots to the left. 
            bulletL.gameObject.tag = "player bullet";                                                           // This newly cloned prefab will be tagged with "player bullet".
            var bulletR = Instantiate(boltPrefab, boltSpawn.transform.position, boltSpawnR.transform.rotation); // Clone the projectile prefab and set the spawn point and rotation for it. This is for the projectile that shoots to the right. 
            bulletR.gameObject.tag = "player bullet";                                                           // This newly cloned prefab will be tagged with "player bullet".
            ammoLevel -= 2;                                         // Decrement the ammunition the player has by 2.                                              
        }
        // Check if the three way shot pickup is activated.
        if (isThreeWay) {            
            time3W += Time.deltaTime;                               // The time since this pickup has been activated.
            // Check if the time the pickup has been activate for is equal to or more than the pickup expire time, which has been set at 10 seconds.
            if (time3W >= secondsofThreeWay) {
                isThreeWay = false;                                 // The three way shot ability becomes deactivated.
                time3W = 0.0f;                                      // Reset the time for the three way shot pickup to zero in the event this needs to be used again.
                StartCoroutine(ThreeWayPickUpOff());                // The text that tells the player this ability is deactivated will appear.
            }
        }

        // Check if the shield pickup is activated.
        if (isShield) {    
            timeShield += Time.deltaTime;                           // The time since this pickup has been activated.
            float expires = 10.0f;                                  // The time this pickup will expire in.
            // Check if the time the pickup has been activate for is equal to or more than the pickups expire time.
            if (timeShield >= expires) {
                shieldPS.Stop();                                    // The shield particle system will stop playing.
                isShield = false;                                   // The shield ability becomes deactivated.
                timeShield = 0.0f;                                  // Reset the time for the shield pickup to zero in the event this needs to be used again.
                StartCoroutine(ShieldPickUpOff());                  // The text that tells the player this ability is deactivated will appear.
            }            
        }

        // Check if the oxygen the player has deducted by the time between frames is more than 0.        
        if (oxygenLevel - Time.deltaTime > 0)
        {
            oxygenLevel -= Time.deltaTime;                          // The oxygen value for the player is deducted by the time between frames. This allows for a gradual decrease for the oxygen.
        }
        else
        {
            oxygenLevel = 0;                                        // Initialise the oxygen variable to zero so it is an absolute number.
        }

        // Check if the oxygen the player has is equal to 0 AND the health of the player is more than 0.     
        if (oxygenLevel == 0 && currentHealth > 0) {
            currentHealth -= Time.deltaTime;                        // The health for the player is deducted by the time between frames. This allows for a gradual decrease for the health.
        } 

        // Check if the health of the player is less than or equal to 0 and is not dead.
        if (currentHealth <= 0 && !isDead) {
            isDead = true;                                          // The player is not confirmed to be dead.
            PlayerDeath();                                          // Execute PlayerDeath(). The process for when the player dies will take effect from here on such as playing of a particle effect and message display.
        }

        // Check if the coins left in the level are 0. This is the win condition for the game. When the set amount of coins have been collected the player completes the level.
        if(remainingCoins == 0) {
            StartCoroutine(Completed());                            // The completed level message will appear on the screen and then the player can enter there name for the highscore table.
        }

        ammoText.text = "Ammo: " + ammoLevel.ToString();                    // The value for the ammunition the player has is displayed in the ammoText text field. This displays on the screen via the GUI camera.
        oxygenText.text = "Oxygen: " + ((int)oxygenLevel).ToString();       // The value for the oxygen the player has is displayed in the oxygenText text field. This displays on the screen via the GUI camera.
        healthText.text = "Health: " + ((int)currentHealth).ToString();     // The value for the health the player has is displayed in the healthText text field. This displays on the screen via the GUI camera.
        scoreText.text = "Score: " + score.ToString();                      // The value for the score the player has is displayed in the scoreText text field. This displays on the screen via the GUI camera.
        coinsText.text = "Coins to find: " + remainingCoins.ToString();     // The number of coins the player has to find is displayed in the coinsText text field. This displays on the screen via the GUI camera.
    }

    void FixedUpdate() {
        // Check if the attract mode is running.
        if (isRunning) {
            animationModel.GetComponent<Animation>().Blend("Run");          // The player will use the "Run" animation from the animationModel game object.
        }
    }
    
    // This method calculates the closest pickup to the player while the attract demo is in progress.
    void ClosestPickUp() {
        agent.speed = 3.5f;                                                 // Set the speed for the player.
        isRunning = true;                                                   // The attract demo is in motion.
        List<GameObject> pickUps = new List<GameObject>();                  // Set up a list for the method.
        pickUps.Add(coinObj);                                               // Add the coin object from the level to the list.
        pickUps.Add(crateObj);                                              // Add the crate object from the level to the list.
        pickUps.Add(oxygenObj);                                             // Add the oxygen object from the level to the list.
        shortestDistance = float.PositiveInfinity;                          // The shortest distance default is initialised.
        closestPowerUp = null;                                              // The closest pickup game object is given a value of null to begin with.
        // The for loop will repeat until it has gone through all elements of the pickUps lists. 
        for (int i = 0; i < pickUps.Count; i++) {
            // Check if the game object in the pickUps list does exist.
            if (pickUps[i] != null) { 
                float distance = Vector3.Distance(transform.position, pickUps[i].transform.position);   // Record the distance from the player to the pickups position.
                // Check if this distance is less than the shortest distance setup as a baseline earlier.
                if (distance < shortestDistance) {
                    shortestDistance = distance;                            // The shortest distance is now the distance just calculated.
                    closestPowerUp = pickUps[i];                            // The closest pickup is set to be the pickup from the current for loop.
                }
            }
        }        

        // Check if this method has performed already.
        if (!closestOnce) {
            closestOnce = true;                                             // This ClosestPickUp() has been executed.
        }        

        agent.destination = closestPowerUp.transform.position;              // The destination for the player is set to the position of the closest pickup which was recently determined.
    }

    // This method creates the pickups for the playable game scene. This method is called when the game has been started by the player by clicking on the 'Play Game' button.
    void CreatePickUps() {
        isStartedFlag = true;               // This indicates that the game has begun the play mode.
        coinObj1.SetActive(true);           // This coin object in the level will now appear in the level.
        coinObj2.SetActive(true);           // This coin object in the level will now appear in the level.
        coinObj3.SetActive(true);           // This coin object in the level will now appear in the level.
        coinObj4.SetActive(true);           // This coin object in the level will now appear in the level.
        coinObj5.SetActive(true);           // This coin object in the level will now appear in the level.
        crateObj1.SetActive(true);          // This crate object in the level will now appear in the level.
        oxygenObj1.SetActive(true);         // This oxygen object in the level will now appear in the level.
    }

    // This method handles the operations for when the player shoots a projectile.
    public void PlayerFire() {

        // A local game object variable is created that is referenced to an instantiated bullet prefab. The position and rotation are set to the bulletSpawn and spine game object.
        var bullet = Instantiate(boltPrefab, boltSpawn.transform.position, spine.transform.rotation);
        bullet.gameObject.tag = "player bullet";                    // Assign the bullet the tag "player bullet".           
    }

    // This method occurs when the player collides with either items or pickups in the level.
    void OnTriggerEnter(Collider other)
    {
        #region Pick-Ups

        // Check if the other collider is tagged with "coin".
        if (other.CompareTag("coin"))
        {
            AudioManager.instance.PlayerPlayClip(coinSound);        // The collected coin sound will play.
            remainingCoins--;                                       // Decrement the coins left to collect for the level by 1.
            score += 50;                                            // The coins value of 50 is added to the players score.

            other.gameObject.GetComponent<Pickup>().Collected();    // Execute Collected() from the Pickup Script. This will destroy the coin object.
        }
        // Check if the other collider is tagged with "oxygen".
        if (other.CompareTag("oxygen"))
        {
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            oxygenLevel = Mathf.Clamp(oxygenLevel+30, 0, oxygenMax);// The oxygen for the player will increase by an additional 30.
            other.gameObject.GetComponent<Pickup>().Collected();    // Execute Collected() from the Pickup Script. This will destroy the oxygen object.
        }
        // Check if the other collider is tagged with "crate".
        if (other.CompareTag("crate"))
        {
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            ammoLevel += 10;                                        // The ammunition for the player will increase by a value of 10.
            other.gameObject.GetComponent<Pickup>().Collected();    // Execute Collected() from the Pickup Script. This will destroy the crate object.
        }
        #endregion        

        #region GunSpecific
        // Check if the other collider is tagged with "heat seek".
        if (other.CompareTag("heat seek")) {            
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            isHeatSeek = true;                                      // The heat seek ability becomes activated.                      
            Destroy(other.gameObject);                              // Destroy the pickup game object.
            StartCoroutine(HeatSeekPickUpOn());                     // The text that tells the player this ability is activated will appear.            
        }
        // Check if the other collider is tagged with "rapid fire".
        if (other.CompareTag("rapid fire")) {
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            isRapidFire = true;                                     // The rapid fire ability becomes activated.  
            Destroy(other.gameObject);                              // Destroy the pickup game object.
            StartCoroutine(RapidFirePickUpOn());                    // The text that tells the player this ability is activated will appear.
        }
        // Check if the other collider is tagged with "three way".
        if (other.CompareTag("three way")) {
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            isThreeWay = true;                                      // The three way ability becomes activated.  
            Destroy(other.gameObject);                              // Destroy the pickup game object.
            StartCoroutine(ThreeWayPickUpOn());                     // The text that tells the player this ability is activated will appear.
        }
        // Check if the other collider is tagged with "explosive".
        if (other.CompareTag("explosive")) {         
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            isExplosive = true;                                     // The explosive ability becomes activated.  
            Destroy(other.gameObject);                              // Destroy the pickup game object.
            StartCoroutine(ExplosivePickUpOn());                    // The text that tells the player this ability is activated will appear.            
        }
        // Check if the other collider is tagged with "shield".
        if (other.CompareTag("shield")) {
            AudioManager.instance.PlayerPlayClip(powerUpSound);     // The collected pickup sound will play.
            isShield = true;                                        // The shield ability becomes activated.  
            Destroy(other.gameObject);                              // Destroy the pickup game object.
            shieldPS.Play();                                        // The shield particle system will start playing.
            StartCoroutine(ShieldPickUpOn());                       // The text that tells the player this ability is activated will appear.
        }
        #endregion     
    }

    #region PickUpDisplay

    // This routine will display text to the player indicating the pickup that has just been activated.
    IEnumerator HeatSeekPickUpOn() {
        heatSeekOn.enabled = true;                  // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        heatSeekOn.enabled = false;                 // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been deactivated.
    IEnumerator HeatSeekPickUpOff() {
        heatSeekOff.enabled = true;                 // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        heatSeekOff.enabled = false;                // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been activated.
    IEnumerator RapidFirePickUpOn() {
        rapidFireOn.enabled = true;                 // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        rapidFireOn.enabled = false;                // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been deactivated.
    IEnumerator RapidFirePickUpOff() {
        rapidFireOff.enabled = true;                // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        rapidFireOff.enabled = false;               // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been activated.
    IEnumerator ThreeWayPickUpOn() {
        threeWayOn.enabled = true;                  // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        threeWayOn.enabled = false;                 // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been deactivated.
    IEnumerator ThreeWayPickUpOff() {
        threeWayOff.enabled = true;                 // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        threeWayOff.enabled = false;                // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been activated.
    IEnumerator ExplosivePickUpOn() {
        explosiveOn.enabled = true;                 // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        explosiveOn.enabled = false;                // Remove the text from the screen.    
    }
    // This routine will display text to the player indicating the pickup that has just been deactivated.
    IEnumerator ExplosivePickUpOff() {
        explosiveOff.enabled = true;                // Make the text visible for the player to see.    
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        explosiveOff.enabled = false;               // Remove the text from the screen.
    }
    // This routine will display text to the player indicating the pickup that has just been activated.
    IEnumerator ShieldPickUpOn() {
        shieldOn.enabled = true;                    // Make the text visible for the player to see.    
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        shieldOn.enabled = false;                   // Remove the text from the screen.
    }   
    // This routine will display text to the player indicating the pickup that has just been deactivated.
    IEnumerator ShieldPickUpOff() {
        shieldOff.enabled = true;                   // Make the text visible for the player to see.
        yield return new WaitForSeconds(2);         // Wait for 2 seconds before continuing with the routine.
        shieldOff.enabled = false;                  // Remove the text from the screen.
    }

    #endregion

    // This method deals with the damage taken when hit by a projectile.
    public void DamageTaken(int damage) {

        // Check if the player is not using a shield.
        if (!isShield) {
            AudioManager.instance.PlayerPlayClip(hurtSound);        // The player has received damaged sound will play.
            currentHealth -= damage;                                // The health of then player deducted by the damage value of the projectile.
        }

        // Check if the health is equal to or less than zero.
        if (currentHealth <= 0) {
            currentHealth = 0;                                      // Initialise the health variable to zero so it is an absolute number.
            AudioManager.instance.PlayerPlayClip(deathSound);       // The player death sound will play.
            PlayerDeath();                                          // Execute PlayerDeath(). The process for when the player dies will take effect from here on such as playing of a particle effect and message display.
        }
    }
    
// This method is called when the player has been killed.
void PlayerDeath() {
        GetComponentInChildren<ParticleSystem>().Play();            // The particle system for when the player dies will be played. These are bubbles as if it is the players last breath.
        GetComponent<CharacterController>().enabled = false;        // This will disable the players ability to move now they are dead.
        StartCoroutine(GameOver());                                 // This will display a game over message to the player.
    }
        
    // This routine will display the game over text to the player.
    IEnumerator GameOver(){
        yield return new WaitForSeconds(2f);                    // Wait for 2 seconds before continuing with the routine.
        userInterface.SetActive(false);                         // Removes the UI from the display.
        gameOverScreen.SetActive(true);                         // The game over screen will become visible to the player here. With the text "Game Over" appearing on the screen.
        yield return new WaitForSeconds(3f);                    // Wait for 3 seconds before continuing with the routine.
        gameOverScreen.SetActive(false);                        // The game over screen will disappear.
        Time.timeScale = 0f;                                    // The game is set to being in a paused state so in the background nothing is moving.
        sceneControl.GetComponent<SceneController>().InputEntry(); // Execute InputEntry() from the SceneController Script. A screen will appear asking for the player to enter their name. 
    }

    // This routine will display the completed level text to the player.
    IEnumerator Completed() {
        userInterface.SetActive(false);                         // Removes the UI from the display.
        completedScreen.SetActive(true);                        // The completed screen will become visible to the player here. With the text "Well done. All coins found" appearing on the screen.
        yield return new WaitForSeconds(3f);                    // Wait for 3 seconds before continuing with the routine.
        completedScreen.SetActive(false);                       // The completed screen will disappear. 
        Time.timeScale = 0f;                                    // The game is set to being in a paused state so in the background nothing is moving.
        sceneControl.GetComponent<SceneController>().InputEntry(); // Execute InputEntry() from the SceneController Script. A screen will appear asking for the player to enter their name. 
    }

}
