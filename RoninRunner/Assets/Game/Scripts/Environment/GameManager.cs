using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Public Variables
    // Game Object variables
    [Header("Main Settings")]
    public GameObject player;
    public GameObject wall;
    public GameObject pauseMenu;
    public GameObject pickupObj;
    public GameObject goodWall;

    // Slow Time variables
    [Header("Slow Down Time Settings")]
    public float slowMultiplier = 0f;

    // More Slow Time variables - souls
    [Header("Soul Duration Settings")]
    public float soulDuration = 1.0f;
    public int MAX_SOULS = 10;
    public int soulCount = 0;

    // Respawn Settings
    // Pickup Respawn Settings
    [Header("Respawn: Pickup Settings")]
    public GameObject[] pickupPrefab;

    // Enemy Respawn Settings
    [Header("Respawn: Enemy Settings")]
    public GameObject enemyObj;
    public GameObject[] platEnemies;
    public GameObject enemyPrefab;

    // Global timeManager variable
    [HideInInspector]
    public TimeManager timeManager;

    // IsSlowed
    [HideInInspector]
    public bool isSlowMode = false;

    // Pickup Variables
    [HideInInspector]
    public Pickup[] pickups;
    [HideInInspector]
    public bool[] pickupDestroyed;
   // [HideInInspector]
    // public AudioClip pickupSound;
    public GameObject pickupSoundPlayer;
    public AudioSource audSource;

    // Enemy Variables
    [HideInInspector]
    public List<EnemyController> enemies;
    [HideInInspector]
    public bool[] enemiesDestroyed;

    // Private Variables
    // Component variables
    private InGameFadeBlack fadeBlack;
    private PlayerMovement playerMovement;
    private WallHazard wallHazard;
    private AudioSource theAudio;

    // Time variables
    private float endTime = 0f;

    // Slow Time variables
    private float slowStartTime = 0f;
    private float slowCurrTime = 0f;
    private float slowElapsedTime = 0f;

    // Pickup Variables
    private string[] pickupName;
    private Quaternion[] pickupRot;
    private Vector3[] pickupPos;

    // Enemies Respawn Variables
    private string[] enemiesName;
    private Quaternion[] enemiesRot; 
    private Vector3[] enemiesPos;
    private Vector3[] enemiesScale;
    private Vector3[] enemiesSightBoundsSize;
    private Vector3[] enemiesAttackBoundsSize;
    private Vector3[] enemiesBoundsOnePos;
    private Vector3[] enemiesBoundsTwoPos;
    private Transform[] enemiesParent;

    // Properties
    public int MAXSOULS { get { return MAX_SOULS; } }

    // Paused variables
    private bool isPaused = false;

    // Tutorial paused variables
    public GameObject tutorialUI;
    private bool isTutorial = false;
    public GameObject currentTutorial = null;
    public TutorialPopup tutorial = null;
    public bool IsTutorial { get { return isTutorial; } set { isTutorial = value; } }

    // Ending Variables
    private bool isEnding = false;
    [HideInInspector]
    public bool IsEnding { get { return isEnding; } }

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        if (player.GetComponent<PlayerMovement>() == true)
            playerMovement = player.GetComponent<PlayerMovement>();
        else
            Debug.LogError("The component, 'PlayerMovement', is missing.");

        if (wall.GetComponent<WallHazard>() == true)
            wallHazard = wall.GetComponent<WallHazard>();
        else
            Debug.LogError("The component, 'WallHazard', is missing.");

        fadeBlack = GetComponent<InGameFadeBlack>();

        if(pickupSoundPlayer.GetComponent<AudioSource>())
        {
            audSource = pickupSoundPlayer.GetComponent<AudioSource>();
        }

        // Pickup Object Info for respawning
        pickups = pickupObj.GetComponentsInChildren<Pickup>();
        pickupDestroyed = new bool[pickups.Length];
        pickupName = new string[pickups.Length];
        pickupRot = new Quaternion[pickups.Length];
        pickupPos = new Vector3[pickups.Length];

        for (int i = 0; i < pickups.Length; i++)
        {
            pickupDestroyed[i] = false;
            pickupName[i] = pickups[i].name;
            pickupRot[i] = pickups[i].transform.rotation;
            pickupPos[i] = pickups[i].transform.position;
        }

        //// Enemy Object Info for respawning
        //EnemyController[] temp = enemyObj.GetComponentsInChildren<EnemyController>();
        //for (int i = 0; i < temp.Length; i++)
        //    enemies.Add(temp[i]);

        //for (int i = 0; i < platEnemies.Length; i++)
        //    enemies.Add(platEnemies[i].GetComponent<EnemyController>());

        //enemiesDestroyed = new bool[enemies.Count];
        //enemiesName = new string[enemies.Count];
        //enemiesRot = new Quaternion[enemies.Count];
        //enemiesPos = new Vector3[enemies.Count];
        //enemiesScale = new Vector3[enemies.Count];
        //enemiesSightBoundsSize = new Vector3[enemies.Count];
        //enemiesAttackBoundsSize = new Vector3[enemies.Count];
        //enemiesBoundsOnePos = new Vector3[enemies.Count];
        //enemiesBoundsTwoPos = new Vector3[enemies.Count];
        //enemiesParent = new Transform[enemies.Count];

        //for (int i = 0; i < enemies.Count; i++)
        //{
        //    enemiesDestroyed[i] = false;
        //    enemiesParent[i] = enemies[i].transform.parent;
        //    enemiesName[i] = enemies[i].name;
        //    enemiesRot[i] = enemies[i].transform.rotation;
        //    enemiesPos[i] = enemies[i].transform.localPosition;
        //    enemiesScale[i] = enemies[i].transform.localScale;
        //    enemiesSightBoundsSize[i] = enemies[i].GetComponent<EnemyController>().sightBounds.GetComponent<BoxCollider>().size;
        //    enemiesAttackBoundsSize[i] = enemies[i].GetComponent<EnemyController>().attackBounds.GetComponent<BoxCollider>().size;
        //    enemiesBoundsOnePos[i] = enemies[i].GetComponent<EnemyController>().bounds[0].transform.position;
        //    enemiesBoundsTwoPos[i] = enemies[i].GetComponent<EnemyController>().bounds[1].transform.position;
        //}

        timeManager = new TimeManager();
        theAudio = GetComponent<AudioSource>();
        if (theAudio.clip != null)
            theAudio.Play();

        endTime += Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (soulCount > MAX_SOULS)
            soulCount = MAX_SOULS;

        // Checks if the 'escape' key has been pressed and if the game is paused
        var p1 = wallHazard._particleSystem;

        #region TUTORIAL

        if (IsTutorial && (Input.GetKeyDown(KeyCode.Mouse0)))
        {
            if (gameObject.tag != "Pickup" && gameObject == currentTutorial)
                gameObject.SetActive(false);
            else if (gameObject.tag == "Pickup" && gameObject == currentTutorial)
                tutorial.hasBeenActivated = true;

            p1 = wallHazard._particleSystem;

            p1.Play();

            tutorialUI.SetActive(false);
            IsTutorial = false;
            timeManager.SetPlayerTimeScale(1);
            timeManager.SetGlobalTimeScale(1);

            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i].rotation = pickups[i].tempRotation;
                pickups[i].startSpeed = pickups[i].tempStartSpeed;
            }
            currentTutorial = null;
            tutorial = null;
        }

        #endregion TUTORIAL

        if (!isPaused && !isTutorial && Input.GetKeyUp(KeyCode.Escape))
        {
            p1 = wallHazard._particleSystem;
            // Pauses the game
            isPaused = !isPaused;
            pauseMenu.SetActive(true);
            p1.Pause();
            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i].tempRotation = pickups[i].rotation;
                pickups[i].tempStartSpeed = pickups[i].startSpeed;

                pickups[i].rotation = new Vector3(0, 0, 0);
                pickups[i].startSpeed = 0;
            }
        }
        else if (isPaused && Input.GetKeyUp(KeyCode.Escape))
        {
            // Unpauses the game
            isPaused = !isPaused;
            pauseMenu.SetActive(false);
            p1.Play();
            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i].rotation = pickups[i].tempRotation;
                pickups[i].startSpeed = pickups[i].tempStartSpeed;
            }
        }

        //Toggle Slowtime
        if(!isPaused && !isTutorial && Input.GetKeyDown(KeyCode.Q) && soulCount > 0)   //Check if paused, player has souls, and pressed q
        {
            if(isSlowMode)  //If time is slowed, speed it back up
            {
                SlowMode(false);
            }
            else     //Slow time
            {
                slowStartTime = Time.deltaTime; //Grab the time the soul started draining at
                SlowMode(true);
            }
            
        }

        //Decrement soul count based on how long time has been slowed in REAL seconds
        if(isSlowMode)
        {
            slowElapsedTime = (float)Math.Round(endTime - slowStartTime, 0);
            if (slowElapsedTime != slowCurrTime && slowElapsedTime % soulDuration == 0)
            {
                if (soulCount > 0)
                    soulCount--;
                else
                    SlowMode(false);

                slowCurrTime = slowElapsedTime;
            }
        }

        // Check for amulet interaction
        RaycastHit h1;
        RaycastHit h2;
        if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.right), out h1, 2))
        {
            if (h1.collider.isTrigger && h1.collider.gameObject.name == "SealStone_Amulet")
            {
                isEnding = true;

                Destroy(h1.collider.gameObject);

                GameObject newWall = Instantiate(goodWall, Vector3.zero, Quaternion.identity);
                newWall.transform.position = new Vector3(2040, -6, 5);
                newWall.transform.rotation = GameObject.FindGameObjectWithTag("Hazard").transform.rotation;

                StartCoroutine(GameEnding());
            }
            else if (Physics.Raycast(player.transform.position, player.transform.TransformDirection(Vector3.left), out h2, 2))
            {
                if (h2.collider.isTrigger && h2.collider.gameObject.name == "SealStone_Amulet")
                {
                    isEnding = true;

                    Destroy(h1.collider.gameObject);

                    GameObject newWall = Instantiate(goodWall, Vector3.zero, Quaternion.identity);
                    newWall.transform.position = new Vector3(2040, -6, 5);
                    newWall.transform.rotation = GameObject.FindGameObjectWithTag("Hazard").transform.rotation;

                    StartCoroutine(GameEnding());
                }
            }
        }

        //Debug.Log(soulCount);
        endTime += Time.deltaTime;
    }

    // Pauses the game via the button in the pause menu
    public void PauseButton()
    {
        var p1 = wallHazard._particleSystem;
        if (!isPaused)
        {
            // Pauses the game
            isPaused = !isPaused;
            pauseMenu.SetActive(true);
            p1.Pause();
            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i].tempRotation = pickups[i].rotation;
                pickups[i].tempStartSpeed = pickups[i].startSpeed;

                pickups[i].rotation = new Vector3(0, 0, 0);
                pickups[i].startSpeed = 0;
            }
        }
        else
        {
            // Unpauses the game
            isPaused = !isPaused;
            pauseMenu.SetActive(false);
            p1.Play();
            for (int i = 0; i < pickups.Length; i++)
            {
                pickups[i].rotation = pickups[i].tempRotation;
                pickups[i].startSpeed = pickups[i].tempStartSpeed;
            }
        }
    }

    public void SlowMode(bool toggle)
    {
        var ps = wallHazard._particleSystem.main;
        if (toggle)
        {
            timeManager.SetGlobalTimeScale(1.0f / slowMultiplier);
            ps.simulationSpeed = 0.1f;
            slowStartTime = endTime;
            isSlowMode = true;
        }
        else
        {
            timeManager.SetGlobalTimeScale(1.0f);
            ps.simulationSpeed = 1.0f;
            isSlowMode = false;
        }    
    }

    // This activates the end of the game
    private IEnumerator GameEnding()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            StartCoroutine(fadeBlack.FadeToBlack(true));
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("EndCutscene");
        }
    }

    // This function respawns all pickups after the latest checkpoint after death
    public void RespawnPickups()
    {
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickupDestroyed[i] && pickupPos[i].x > player.GetComponent<Health>().respawnPos.x)
            {
                string theName = "";
                if (pickupName[i][pickupName[i].Length - 1] == ')')
                {
                    theName = pickupName[i].Split('(')[0].ToString();
                    if (theName[theName.Length - 1] == ' ')
                        theName = theName.Split(' ')[0].ToString();
                }
                else
                    theName = pickupName[i];

                GameObject pickup = null;
                for (int j = 0; j < pickupPrefab.Length; j++)
                {
                    if (pickupPrefab[j].name == theName)
                        pickup = pickupPrefab[j];
                }

                if (pickup != null)
                {
                    UnityEngine.Object temp = Instantiate(pickup, pickupPos[i], pickupRot[i], pickupObj.transform);
                    GameObject newPickup = (GameObject)temp;

                    newPickup.name = pickupName[i];
                    newPickup.GetComponent<Pickup>().collected = false;
                    pickupDestroyed[i] = false;
                }
            }
        }
    }

    // This function respawns enemies after the latest checkpoint after death
    public void RespawnEnemies()
    {
        //for (int i = 0; i < enemies.Count; i++)
        //{
        //    if (enemiesDestroyed[i] && enemiesPos[i].x > player.GetComponent<Health>().respawnPos.x)
        //    {
        //        GameObject enemy = enemyPrefab;
        //        if (enemy != null)
        //        {
        //            GameObject newEnemy = Instantiate(enemy, enemiesPos[i], enemiesRot[i], enemiesParent[i]);

        //            enemiesDestroyed[i] = false;

        //            newEnemy.name = enemiesName[i];
        //            newEnemy.transform.localScale = enemiesScale[i];
        //            newEnemy.transform.localPosition = enemiesPos[i];
        //            newEnemy.GetComponent<EnemyController>().sightBounds.GetComponent<BoxCollider>().size = enemiesSightBoundsSize[i];
        //            newEnemy.GetComponent<EnemyController>().attackBounds.GetComponent<BoxCollider>().size = enemiesAttackBoundsSize[i];
        //            newEnemy.GetComponent<EnemyController>().bounds[0].transform.position = enemiesBoundsOnePos[i];
        //            newEnemy.GetComponent<EnemyController>().bounds[1].transform.position = enemiesBoundsTwoPos[i];
        //        }
        //    }
        //}
    }

    // This exits the game
    public void ExitGame()
    {
        StartCoroutine(EndGame());
    }

    public IEnumerator EndGame()
    {
        while (true)
        {
            StartCoroutine(fadeBlack.FadeToBlack(true));
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("MainMenu");
        }
    }

    // This gets the 'isPaused' variable
    public bool GetIsPaused()
    {
        return isPaused;
    }

    // This sets the 'isPaused' variable
    public void SetIsPaused(bool pause)
    {
        isPaused = pause;
    }
}