using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatforms : MonoBehaviour
{
    // Public variables
    public float initialSpeed = 3f;
    public List<GameObject> platforms;
    public string[] affectedTags;

    // Private variables
    // Game Manager variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;

    // RotatingPlatform variables
    private List<TriggerParent> hitboxes = new List<TriggerParent>();
    private Transform hit;
    private bool hasBeenHit = false;
    private float speed;
    private Quaternion tempRot = new Quaternion(0, 0, 0, 0);

    // Target Variables
    private float offset = 0f;
    private float newPos = 0f;
    private float oldPos = 0f;
    private bool stickToY = false;

    // Start is called before the first frame update
    private void Start()
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

        for (int i = 0; i < platforms.Count; i++)
        {
            hitboxes.Reverse();
            hitboxes.Add(platforms[i].GetComponentInChildren<TriggerParent>());
            hitboxes.Reverse();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            stickToY = false;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        isPaused = gameManager.GetIsPaused();

        if (!isPaused && !gameManager.IsTutorial)
        {
            speed = initialSpeed * Time.deltaTime * (gameManager.timeManager.GetGlobalTimeScale());
            transform.Rotate(0, 0, speed);

            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].transform.Rotate(0, 0, -speed);
            }
        }
    }
}