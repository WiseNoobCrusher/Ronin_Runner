using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxPair
{
    public List<TriggerParent> enter;
    public List<TriggerParent> exit;
}

[RequireComponent(typeof(WallHazard))]
public class WallSpeedHandler : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Component Variables
    [Header("Main Settings")]
    public PlayerMovement player;

    // Float Variables
    public float minDistanceAway = 5f;
    public float maxDistanceAway = 30f;

    // GameObject Variables
    public GameObject[] hitboxes;

    // Non-Inspector Variables
    // Bool Variables
    [HideInInspector]
    public bool withinTrigger = false;

    // Private Variables
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused = false;

    // GameObject Variables
    private List<GameObject> pos;
    private GameObject currPos;

    // Component Variables
    private WallHazard wallHazard;

    // HitboxPair Variables
    private List<HitboxPair> hitboxPairs;

    // Float Variables
    private float turn = 0;
    private float currDir = -1;
    private float tempSpeed = 0;

    // Bool Variables
    private bool hasEntered = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameManager") == true)
        {
            gameManagerObj = GameObject.FindGameObjectWithTag("GameManager");
            if (gameManagerObj.GetComponent<GameManager>() == true)
            {
                gameManager = gameManagerObj.GetComponent<GameManager>();
                isPaused = gameManager.GetIsPaused();
            }
            else
                Debug.LogError("The component 'GameManager' does not exist.");
        }
        else
            Debug.LogError("No object with the tag 'GameManager' exists.");

        wallHazard = GetComponent<WallHazard>();

        pos = new List<GameObject>();

        hitboxPairs = new List<HitboxPair>();
        for (int i = 0; i < hitboxes.Length; i++)
        {
            HitboxPair pair = new HitboxPair();

            pair.enter = new List<TriggerParent>();
            pair.exit = new List<TriggerParent>();

            WallParent[] temp = hitboxes[i].GetComponentsInChildren<WallParent>();
            for (int j = 0; j < temp.Length; j++)
            {
                pair.enter.Add(temp[j].hitIn);
                pair.exit.Add(temp[j].hitOut);
            }

            hitboxPairs.Add(pair);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = gameManager.GetIsPaused();

        if (!isPaused)
        {
            if (withinTrigger)
            {
                if (wallHazard.disBetweenTargets < maxDistanceAway)
                    wallHazard.speed = tempSpeed * 0.02f;
                else
                {
                    if (wallHazard.disBetweenTargets < minDistanceAway)
                        wallHazard.speed = tempSpeed * 0.005f;
                }
            }
            else
                tempSpeed = wallHazard.speed;

            UpdateSpeed();
            UpdateTriggers();
        }
    }

    public void UpdateSpeed(bool bypass = false)
    {
        if (!withinTrigger || bypass)
        {
            if (player.isMoving)
            {
                if (player.faceDirection > 0)
                {
                    if (wallHazard.disBetweenTargets > maxDistanceAway)
                        wallHazard.speed = player.speed + (player.speed * 1.75f);
                    else
                    {
                        if (wallHazard.disBetweenTargets < minDistanceAway)
                            wallHazard.speed = player.speed + (player.speed * 0.01f);
                        else
                            wallHazard.speed = player.speed + (player.speed * 0.025f);
                    }
                }
                else
                {
                    if (wallHazard.disBetweenTargets > maxDistanceAway)
                        wallHazard.speed = player.speed + (player.speed * 1.75f);
                    else
                    {
                        if (wallHazard.disBetweenTargets < minDistanceAway)
                            wallHazard.speed = player.speed + (player.speed * 0.01f);
                        else
                            wallHazard.speed = player.speed + (player.speed * 0.025f);
                    }
                }
            }
            else
            {
                if (wallHazard.disBetweenTargets > maxDistanceAway)
                    wallHazard.speed = wallHazard.initialSpeed + (wallHazard.initialSpeed * 1.75f);
                else
                {
                    if (wallHazard.disBetweenTargets < minDistanceAway)
                        wallHazard.speed = wallHazard.initialSpeed + (wallHazard.initialSpeed * 0.1f);
                    else
                        wallHazard.speed = wallHazard.initialSpeed + (wallHazard.initialSpeed * 0.3f);
                }
            }
        }

        //Debug.Log("Wall Speed: " + wallHazard.speed.ToString());
        //Debug.Log("Player Speed: " + player.speed.ToString());
        //Debug.Log("Distance from Wall: " + wallHazard.disBetweenTargets.ToString());
    }

    private void UpdateTriggers()
    {
        for (int i = 0; i < hitboxPairs.Count; i++)
        {
            if (pos.Count <= 0 && !hasEntered)
            {
                for (int j = 0; j < hitboxPairs[i].enter.Count; j++)
                {
                    var collider = hitboxPairs[i].enter[j];
                    if (collider.colliding && collider.hitObject != null)
                    {
                        pos.Add(collider.gameObject);
                        hasEntered = true;
                    }
                }
            }
            else
            {
                for (int j = 0; j < hitboxPairs[i].enter.Count; j++)
                {
                    var enterCollider = hitboxPairs[i].enter[j];
                    var exitCollider = hitboxPairs[i].exit[j];

                    // Checks if player has collided with the enter collider
                    if (enterCollider.colliding && enterCollider.hitObject != null)
                    {
                        if (!pos.Contains(enterCollider.gameObject))
                            pos.Add(enterCollider.gameObject);
                    }
                    else
                    {
                        if (pos.Contains(enterCollider.gameObject))
                            pos.Remove(enterCollider.gameObject);
                    }

                    // Checks if player has collided with the exit collider
                    if (exitCollider.colliding && exitCollider.hitObject != null)
                    {
                        if (!pos.Contains(exitCollider.gameObject))
                            pos.Add(exitCollider.gameObject);
                    }
                    else
                    {
                        if (pos.Contains(exitCollider.gameObject))
                            pos.Remove(exitCollider.gameObject);
                    }

                    if (pos.Count == 1)
                    {
                        currPos = pos[0];

                        if (currPos.name == "In")
                        {
                            withinTrigger = true;
                            Debug.Log("In");
                        }
                        else if (currPos.name == "Out")
                        {
                            wallHazard.speed = tempSpeed;
                            withinTrigger = false;
                            Debug.Log("Out");
                        }

                        currPos = null;
                        hasEntered = false;
                    }
                    else
                    {
                        currPos = null;
                        hasEntered = false;
                    }
                }
            }
        }
    }
}
