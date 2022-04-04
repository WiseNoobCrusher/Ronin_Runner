using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallJump : MonoBehaviour
{
    // ---------------------------------------------------------
    // Public Variables
    // ---------------------------------------------------------
    // Inspector Variables
    [Header("Main Settings")]
    // Component Variables
    public PlayerMovement playerMovement;

    // Float Variables
    public float gravitySubstracter = 2f;

    // ---------------------------------------------------------
    // Private Variables
    // ---------------------------------------------------------
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;

    // GameObject Variables
    private GameObject currWall = null;
    private GameObject prevWall = null;

    // Transform Variables
    private Transform[] walls;

    // Component Variables
    private List<TriggerParent> hitboxes;

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

        hitboxes = new List<TriggerParent>();
        TriggerParent[] temp = GetComponentsInChildren<TriggerParent>();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].transform.parent == transform)
            {
                Debug.Log(temp[i].name);
                hitboxes.Add(temp[i]);
            }
        }

        walls = GetComponentsInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        currWall = playerMovement.currWall;
        for (int i = 0; i < walls.Length; i++)
        {
            if (walls[i].gameObject == currWall && prevWall != currWall)
            {
                playerMovement.gravity = playerMovement.initialGravity / gravitySubstracter;
                playerMovement.isOnWallPlat = true;

                break;
            }
            else if (i == walls.Length - 1)
            {
                playerMovement.gravity = playerMovement.initialGravity;
                playerMovement.isOnWallPlat = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
            prevWall = currWall;
    }
}
