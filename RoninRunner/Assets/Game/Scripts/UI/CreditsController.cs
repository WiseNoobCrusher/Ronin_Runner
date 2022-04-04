using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    // Public Variables
    // Inspector Variables
    // Credits Variables
    [Header("Main Settings")]
    public AudioClip creditsMusic;
    public GameObject startPoint;
    public GameObject endPoint;
    public float speed;

    // Private Variables
    // Time Variables
    private float startTime = 0f;
    private float endTime = 0f;
    private float elapsedTime = 0f;

    // Credits Variables
    private bool isValid = true;
    private SceneManager scene;

    // Start is called before the first frame update
    private void Start()
    {
        if (startPoint == null)
        {
            isValid = false;
            Debug.LogError("'startPoint' is missing, please add a start point.");
        }
        else if (endPoint == null)
        {
            isValid = false;
            Debug.LogError("'endPoint' is missing, please add a end point.");
        }

        if (isValid)
        {
            var startPointPos = startPoint.transform.position;
            startPointPos.y = Screen.height;

            startPoint.transform.position = startPointPos;
        }

        if (creditsMusic)
            AudioSource.PlayClipAtPoint(creditsMusic, Vector3.zero);

        endTime = Time.deltaTime;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (isValid)
        {
            if (endPoint.transform.position.y < startPoint.transform.position.y)
            {
                Vector3 creditsPosition = transform.position;
                creditsPosition.y += speed * Time.deltaTime;

                transform.position = creditsPosition;
            }
            else
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);

        endTime += Time.deltaTime;
    }
}
