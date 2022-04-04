using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodWallController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Movement Variables
    public float initialSpeed = 2f;

    // Private Variables
    // GameManager Settings
    // GameManager Variables
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;

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

        StartCoroutine(UpdateWall());
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = gameManager.GetIsPaused();
    }

    private IEnumerator UpdateWall()
    {
        while (true)
        {
            while (!isPaused)
            {
                var wallPos = transform.position;
                wallPos.x -= initialSpeed * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale());
                transform.position = wallPos;

                yield return null;
            }

            yield return null;
        }
    }
}
