using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmuletController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Bobbing Variables
    [Header("Bobbing Settings")]
    public GameObject amulet = null;
    public float initialBobSpeed = 40f;
    public GameObject[] bobPoints;

    // Spin Variables
    [Header("Spin Settings")]
    public float initialSpinSpeed = 60f;

    // Private Variables
    // GameManager Settings
    private GameObject gameManagerObj;
    private GameManager gameManager;
    private bool isPaused;

    // Main Variables
    private bool isValid = false;

    // Bobbing Variables
    private GameObject currPoint = null;

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

        if (amulet != null)
            isValid = true;
        else
        {
            isValid = false;
            Debug.LogError("The amulet is missing.");
        }

        if (bobPoints.Length == 2)
        {
            isValid = true;
            currPoint = bobPoints[0];

            for (int i = 0; i < bobPoints.Length; i++)
                bobPoints[i].transform.SetParent(null);
        }
        else
        {
            isValid = false;
            Debug.LogError("There is none /or over the limit of points for the amulet to bob from.");
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        isPaused = gameManager.GetIsPaused();

        if (!isPaused)
        {
            if (isValid)
            {
                var amuletPos = transform.position;

                // Bobbing
                amuletPos = Vector3.MoveTowards(amuletPos, currPoint.transform.position, initialBobSpeed * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale()));
                transform.position = amuletPos;

                // Rotate
                amulet.transform.Rotate(new Vector3(0, 0, initialSpinSpeed * (Time.deltaTime * gameManager.timeManager.GetGlobalTimeScale())));

                // Swapping current point
                float distance = Vector3.Distance(transform.position, currPoint.transform.position);
                if (distance <= 0.01f)
                {
                    if (currPoint == bobPoints[0])
                        currPoint = bobPoints[1];
                    else
                        currPoint = bobPoints[0];
                }
            }
        }
    }
}
